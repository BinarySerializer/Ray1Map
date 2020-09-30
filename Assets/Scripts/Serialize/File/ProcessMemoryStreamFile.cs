using System.IO;
using System.Linq; // For Where and OfType
using BinaryTools.Elf; // To find symbols in Linux EXEs
using BinaryTools.Elf.Io; // Make sure it has a reader it's happy with...
using PE; // To find symbols in Windows EXEs
using UnityEngine;

namespace R1Engine.Serialize
{
    public class ProcessMemoryStreamFile : BinaryFile 
    {
        public ProcessMemoryStreamFile(string name, string processFileName, Context context) : base(context)
        {
            ProcessFileName = processFileName;
            filePath = name;
            stream = null;
        }

        private ProcessMemoryStream stream;
        private long baseStreamOffset;

        public string ProcessFileName { get; }

        public long BaseStreamOffset
        {
            get => baseStreamOffset;
            set
            {
                baseStreamOffset = value;

                Debug.Log($"Set memory stream base offset to 0x{BaseStreamOffset:X8}");

                if (stream != null)
                    stream.BaseStreamOffset = value;
            }
        }

        public ProcessMemoryStream GetStream() => stream ?? (stream = new ProcessMemoryStream(ProcessFileName, ProcessMemoryStream.Mode.AllAccess)
        {
            BaseStreamOffset = BaseStreamOffset
        });
        
        public Pointer GetPointerByName(string name) {
            GetStream();

            var exestream = new FileStream(stream.ExeFile, FileMode.Open, FileAccess.Read);
            try {
                // This is really easy for ELF executables.
                var exereader = new EndianBinaryReader(exestream, EndianBitConverter.NativeEndianness);

                ElfFile elfFile = ElfFile.ReadElfFile(exereader);
                foreach (var symbolTable in elfFile.Sections.OfType<ElfSymbolTable>()) {
                    // BinaryTools.Elf's internal code to populate the "Name" property doesn't always work,
                    // it seems, so we need to look up the symbol names in the string table manually.
                    var stringTable = elfFile.Sections[(int)symbolTable.Link];
                    foreach (var foundSymbol in symbolTable.Where(sym => exereader.ReadELFString(stringTable, sym.NameIndex).Equals(name))) {
                        uint ptr = (uint)foundSymbol.Value;
                        if (foundSymbol.Binding == ElfSymbolBinding.Local)
                            ptr += (uint)stream.BaseAddress;
                        return new Pointer(ptr, this);
                    }
                }

                throw new FileNotFoundException($"Symbol {name} not found in {stream.ExeFile}");
            } catch (FileFormatException) {
                exestream.Seek(0, SeekOrigin.Begin);
                // Not ELF. Try PE instead.
                var result = PortableExecutable.TryReadHeader(exestream, out var peFile, true);
                if (result == ReaderError.NoError) {
                    var header = peFile.FileHeader;
                    if (header.PointerToSymbolTable == 0)
                        // TODO: Figure out how to read symbols other than COFF symbols?
                        throw new FileNotFoundException($"{stream.ExeFile} doesn't contain a COFF symbol table");

                    // May need the string table to get long symbol names.
                    long strTabPos = header.PointerToSymbolTable + 18*header.NumberOfSymbols;
                    var symReader = new Reader(exestream);
                    foreach (uint symPos in Enumerable.Range(0,(int)header.NumberOfSymbols).Select(i => header.PointerToSymbolTable + 18*i)) {
                        exestream.Seek(symPos, SeekOrigin.Begin);

                        string symName;
                        if (symReader.ReadUInt32() == 0) {
                            // First four bytes are zero => next four bytes are an offset into the string table.
                            exestream.Seek(strTabPos + symReader.ReadUInt32(), SeekOrigin.Begin);
                            symName = symReader.ReadNullDelimitedString();
                        } else {
                            // First four bytes not zero => the name is eight characters (or less) and right here.
                            exestream.Seek(symPos, SeekOrigin.Begin);
                            symName = symReader.ReadString(8);
                        }

                        if (symName.Equals($"_{name}") // COFF symbols have a preceding underscore, at least in my MinGW Dosbox build.
                                || symName.Equals(name)) { // Try without as well, just in case...
                            exestream.Seek(symPos + 8, SeekOrigin.Begin);
                            uint symValue = symReader.ReadUInt32();

                            // The pointer is relative to the section head.
                            var symSection = peFile.ImageSectionHeaders[symReader.ReadUInt16() - 1]; // Sections are indexed beginning from 1!
                            return new Pointer(peFile.OptionalHeader32.ImageBase + symSection.VirtualAddress + symValue, this);
                        }
                    }
                    throw new FileNotFoundException($"Symbol {name} not found in {stream.ExeFile}");
                } else 
                    throw new InvalidDataException($"{stream.ExeFile} could not be parsed as a PE file ({result})");
            }
        }
        public override Pointer GetPointer(uint serializedValue, Pointer anchor = null) {
            if(serializedValue == 0) return null;
            Pointer ptr = new Pointer(serializedValue, this, anchor: anchor);
            return ptr;
        }

        public override Pointer StartPointer => new Pointer((uint)baseAddress, this);
		public override Reader CreateReader() {
			Reader reader = new Reader(new BufferedStream(new NonClosingStreamWrapper(GetStream())), isLittleEndian: Endianness == Endian.Little);
			return reader;
		}

		public override Writer CreateWriter() {
			Writer writer = new Writer(new BufferedStream(new NonClosingStreamWrapper(GetStream())), isLittleEndian: Endianness == Endian.Little);
			return writer;
		}

		public override void Dispose() {
			stream?.Dispose();
			stream = null;
			base.Dispose();
		}
	}
}

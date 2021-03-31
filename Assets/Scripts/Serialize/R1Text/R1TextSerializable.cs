using System;
using System.IO;
using System.Text;
using BinarySerializer;
using Newtonsoft.Json;


namespace R1Engine
{
    public abstract class R1TextSerializable
    {
        [JsonIgnore]
        protected bool isFirstLoad = true;
        [JsonIgnore]
        public Context Context { get; protected set; }

        public void Serialize(string path, Context context, bool read, Encoding encoding)
        {
            // Set the context
            Context = context;

            if (read)
            {
                using (var s = FileSystem.GetFileReadStream(context.BasePath + path))
                {
                    if (s != null && s.Length > 0)
                    {
                        OnPreSerialize(path);

                        using (R1TextParser parser = new R1TextParser(context.GetR1Settings(), s, encoding))
                            Read(parser);

                        OnPostSerialize(path);
                        isFirstLoad = false;
                    }
                }
            }
            else
            {
                using (var s = FileSystem.GetFileWriteStream(context.BasePath + path))
                {
                    if (s != null)
                    {
                        OnPreSerialize(path);

                        using (R1TextParser parser = new R1TextParser(context.GetR1Settings(), s, encoding))
                            Write(parser);

                        OnPostSerialize(path);
                        isFirstLoad = false;
                    }
                }
            }
        }

        protected virtual void OnPreSerialize(string path) { }
        protected virtual void OnPostSerialize(string path) { }

        public abstract void Read(R1TextParser parser);
        public virtual void Write(R1TextParser parser) => throw new NotImplementedException();
    }
}
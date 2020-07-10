using System;
using System.IO;
using R1Engine.Serialize;

namespace R1Engine
{
    public abstract class MapperTextSerializable
    {
        protected bool isFirstLoad = true;
        public Context Context { get; protected set; }

        public void Serialize(string path, Context context, bool read)
        {
            // Set the context
            Context = context;

            if (read)
            {
                Stream s = context.GetFileStream(path);

                if (s != null && s.Length > 0)
                {
                    OnPreSerialize(path);
                    
                    using (MapperTextParser parser = new MapperTextParser(s))
                        Read(parser);

                    OnPostSerialize(path);
                    isFirstLoad = false;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        protected virtual void OnPreSerialize(string path) { }
        protected virtual void OnPostSerialize(string path) { }

        public abstract void Read(MapperTextParser parser);
        public virtual void Write(MapperTextParser parser) => throw new NotImplementedException();
    }
}
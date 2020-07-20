using System;
using System.IO;
using System.Text;
using R1Engine.Serialize;

namespace R1Engine
{
    public abstract class R1TextSerializable
    {
        protected bool isFirstLoad = true;
        public Context Context { get; protected set; }

        public void Serialize(string path, Context context, bool read, Encoding encoding)
        {
            // Set the context
            Context = context;

            if (read)
            {
                Stream s = context.GetFileStream(path);

                if (s != null && s.Length > 0)
                {
                    OnPreSerialize(path);
                    
                    using (R1TextParser parser = new R1TextParser(context.Settings, s, encoding))
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

        public abstract void Read(R1TextParser parser);
        public virtual void Write(R1TextParser parser) => throw new NotImplementedException();
    }
}
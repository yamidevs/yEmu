using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yEmu.World.Core.Classes
{
    public class Channels
    {
        public List<Channel> Chans { get; set; }

        public Channels()
        {
            Chans = new List<Channel>();

            Add('*', true);
            Add('#', true);
            Add('$', true);
            Add('p', true);
            Add('%', true);
            Add('i', true);
            Add(':', true);
            Add('?', true);
            Add('!', true);
            Add('^', true);
        }

        public void Add(char head, bool state)
        {
            lock (Chans)
                Chans.Add(new Channel(head, state));
        }

        public string Send()
        {
            return string.Concat("cC+", string.Join("", from c in Chans select c.Head.ToString()));
        }

        public string Change(char head, bool state)
        {
            if (Chans.Any(x => x.Head == head))
            {
                Chans.First(x => x.Head == head).On = state;
                return string.Format("cC{0}{1}", (state ? "+" : "-"), head.ToString());
            }
            return "";
        }


        public class Channel
        {
            public char Head { get; set; }
            public bool On { get; set; }

            public Channel(char head, bool on)
            {
                Head = head;
                On = on;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Playlist;

namespace NiconicomeTest.Stabs.Models.Playlist
{
    class VideoInfoContainerStab:IVideoInfoContainer
    {
        public IListVideoInfo GetVideo(string id)
        {
            return new NonBindableListVideoInfo() { NiconicoId = new Reactive.Bindings.ReactiveProperty<string>(id) };
        }

        public void Clear()
        {

        }

        public int Count { get => 0; }
    }
}

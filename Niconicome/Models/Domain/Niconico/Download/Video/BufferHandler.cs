using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Download.Video
{
    public interface IBufferHandler
    {
        int MaxBufferCount { get; }
        int CurrentRetainingBuffersCount { get; }
        bool CanRetailMoreBuffer { get; }
        IBufferData GetFirstBuffer();
        IBufferData GetBufferAt(int index);
        void RemoveBufferAt(int index);
        void StoreBuffer(IBufferData data);
        void ClearBuffer();
    }

    /// <summary>
    /// バッファデータ
    /// </summary>
    public interface IBufferData
    {
        byte[] Buffer { get; set; }
        int SequenceZero { get; }
    }

    /// <summary>
    /// バッファを取り扱う
    /// </summary>
    class BUfferHandler : IBufferHandler
    {

        public BUfferHandler()
        {
            //最大バッファ数(規定値:5)
            const int maxBufferCount = 5;

            this.MaxBufferCount = maxBufferCount;

        }

        /// <summary>
        /// 内部でデータを保持する
        /// </summary>
        private readonly List<IBufferData> bufferData = new();

        /// <summary>
        /// 最大バッファ数
        /// </summary>
        public int MaxBufferCount { get; init; }

        /// <summary>
        /// 現在保持しているバッファの数
        /// </summary>
        public int CurrentRetainingBuffersCount => this.bufferData.Count;

        /// <summary>
        /// 更にバッファを保持できるかどうか
        /// </summary>
        public bool CanRetailMoreBuffer => this.CurrentRetainingBuffersCount < this.MaxBufferCount;

        /// <summary>
        /// 最初のバッファを取得する
        /// </summary>
        /// <returns></returns>
        public IBufferData GetFirstBuffer()
        {
            if (this.CurrentRetainingBuffersCount == 0) throw new InvalidOperationException("現在バッファを保持していないため取得できません。");
            var data = this.bufferData.First();
            return data;
        }

        /// <summary>
        /// 指定したインデックスのバッファを取得する
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IBufferData GetBufferAt(int index)
        {
            int current = this.CurrentRetainingBuffersCount;
            if (current < index + 1) throw new InvalidOperationException($"現在のバッファ保持数は{current}のため、指定したインデックス({index})のデータを取得できません。");
            var data = this.bufferData[index];
            return data;
        }

        /// <summary>
        /// バッファを追加する
        /// </summary>
        /// <param name="data"></param>
        public void StoreBuffer(IBufferData data)
        {
            if (!this.CanRetailMoreBuffer) throw new InvalidOperationException($"現在のバッファ保持数({this.CurrentRetainingBuffersCount})が最大値({this.MaxBufferCount})に達しているためデータを追加できません。");
            this.bufferData.Add(data);
        }

        /// <summary>
        /// バッファをクリアする
        /// </summary>
        public void ClearBuffer()
        {
            foreach (int i in Enumerable.Range(0, this.CurrentRetainingBuffersCount))
            {
                this.bufferData[i].Buffer = Array.Empty<byte>();
            }
            this.bufferData.Clear();
        }

        /// <summary>
        /// 指定した位置のバッファを削除する
        /// </summary>
        /// <param name="index"></param>
        public void RemoveBufferAt(int index)
        {
            int current = this.CurrentRetainingBuffersCount;
            if (current < index + 1) throw new InvalidOperationException($"現在のバッファ保持数は{current}のため、指定したインデックス({index})のデータを削除できません。");
            this.bufferData[index].Buffer = Array.Empty<byte>();
            this.bufferData.RemoveAt(index);
        }

    }

    /// <summary>
    /// バッファデータ
    /// </summary>
    public class Bufferdata : IBufferData
    {

        public Bufferdata(byte[] data, int sequenceZero)
        {
            this.Buffer = data;
            this.SequenceZero = sequenceZero;
        }

        public byte[] Buffer { get; set; }

        public int SequenceZero { get; init; }
    }
}

using System;
using System.IO;
using Captura.Audio;
using IAudioEncoder = SharpAvi.Codecs.IAudioEncoder;

namespace Captura.Models
{
    /// <summary>
    /// Enables a Screna based Audio Provider to be used with SharpAvi.
    /// </summary>
    class IAudioProviderAdapter : IAudioEncoder
    {
        readonly IAudioProvider _provider;

        public IAudioProviderAdapter(IAudioProvider Provider)
        {
            _provider = Provider;
        }

        public int BitsPerSample => _provider.WaveFormat.BitsPerSample;

        public int BytesPerSecond => _provider.WaveFormat.AverageBytesPerSecond;

        public int ChannelCount => _provider.WaveFormat.Channels;

        public short Format => (short)_provider.WaveFormat.Encoding;

        public byte[] FormatSpecificData
        {
            get
            {
                var extraSize = _provider.WaveFormat.ExtraSize;

                if (extraSize <= 0)
                    return null;

                using var ms = new MemoryStream();
                using var writer = new BinaryWriter(ms);
                _provider.WaveFormat.Serialize(writer);

                var formatData = new byte[extraSize];

                ms.Seek(18, SeekOrigin.Begin);

                ms.Read(formatData, 0, extraSize);

                return formatData;
            }
        }

        public int Granularity => _provider.WaveFormat.BlockAlign;

        public int SamplesPerSecond => _provider.WaveFormat.SampleRate;

        public int EncodeBlock(byte[] Source, int SourceOffset, int SourceCount, byte[] Destination, int DestinationOffset)
        {
            Array.Copy(Source, SourceOffset, Destination, DestinationOffset, SourceCount);

            return SourceCount;
        }
        //after updating SharpAvi nuget package interface need this methods
        public int EncodeBlock(ReadOnlySpan<byte> source, Span<byte> destination)
        {
            throw new NotImplementedException();
        }

        public int Flush(byte[] Destination, int DestinationOffset) => 0;

        //after updating SharpAvi nuget package interface need this methods
        public int Flush(Span<byte> destination)
        {
            throw new NotImplementedException();
        }

        public int GetMaxEncodedLength(int SourceCount) => SourceCount;
    }
}
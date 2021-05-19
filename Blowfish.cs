using System.Collections.Generic;
using System.Linq;

namespace itsec_lab4
{
    public class Blowfish
    {
        private readonly SPblocks _sPblocks;
        private int _shiftSize;
        private const byte Filler = 0;
        private const int Rounds = 16;

        public Blowfish(List<byte> key)
        {
            _sPblocks = new SPblocks();

            //расширение ключа
            uint keyInd = 0;

            for (int i = 0, k = 0; i < 18; i++)
            {
                for (int j = 0; j < 4; j++, k++)
                {
                    keyInd = (keyInd << 8) | key[k % key.Count];
                }

                _sPblocks.P[i] ^= keyInd;
            }

            ulong initValL = 0x00000000;
            ulong initValR = 0x00000000;

            for (int i = 0; i < 18; i++)
            {
                Encipher(ref initValR, ref initValL);
                _sPblocks.P[i] = initValR;
                _sPblocks.P[++i] = initValL;
            }

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    Encipher(ref initValR, ref initValL);
                    _sPblocks.S[i, j] = initValR;
                    _sPblocks.S[i, ++j] = initValL;
                }
            }
        }

        public void Encrypt(List<byte> message)
        {
            _shiftSize = 8 - message.Count % 8;
            message.AddRange(Enumerable.Repeat(Filler, _shiftSize));

            var length = message.Count;

            for (int i = 0; i < length; i += 8)
            {
                ulong right = (uint) ((message[i] << 24) | (message[i + 1] << 16) | (message[i + 2] << 8) | message[i + 3]);
                ulong left = (uint) ((message[i + 4] << 24) | (message[i + 5] << 16) | (message[i + 6] << 8) |
                                     message[i + 7]);
                Encipher(ref right, ref left);
                message[i] = (byte) (right >> 24);
                message[i + 1] = (byte) (right >> 16);
                message[i + 2] = (byte) (right >> 8);
                message[i + 3] = (byte) (right);
                message[i + 4] = (byte) (left >> 24);
                message[i + 5] = (byte) (left >> 16);
                message[i + 6] = (byte) (left >> 8);
                message[i + 7] = (byte) (left);
            }
        }

        public void Decrypt(List<byte> message)
        {
            var length = message.Count;
            for (int i = 0; i < length; i += 8)
            {
                ulong right = (uint) ((message[i] << 24) | (message[i + 1] << 16) | (message[i + 2] << 8) | message[i + 3]);
                ulong left = (uint) ((message[i + 4] << 24) | (message[i + 5] << 16) | (message[i + 6] << 8) |
                                     message[i + 7]);
                Decipher(ref right, ref left);
                message[i] = (byte) (right >> 24);
                message[i + 1] = (byte) (right >> 16);
                message[i + 2] = (byte) (right >> 8);
                message[i + 3] = (byte) (right);
                message[i + 4] = (byte) (left >> 24);
                message[i + 5] = (byte) (left >> 16);
                message[i + 6] = (byte) (left >> 8);
                message[i + 7] = (byte) (left);
            }
        }

        private ulong F(ulong x)
        {
            return ((_sPblocks.S[0, (x >> 24) & 0xFF] + _sPblocks.S[1, (x >> 16) & 0xFF]) ^
                    _sPblocks.S[2, (x >> 8) & 0xFF]) + _sPblocks.S[3, x & 0xFF];
        }

        void swap(ref ulong a, ref ulong b)
        {
            var temp = a;
            a = b;
            b = temp;
        }

        private void Decipher(ref ulong left, ref ulong right)
        {
            for (int i = Rounds + 1; i > 1; --i)
            {
                left = left ^ _sPblocks.P[i];
                right = F(left) ^ right;

                swap(ref left, ref right);
            }

            swap(ref left, ref right);

            right = right ^ _sPblocks.P[1];
            left = left ^ _sPblocks.P[0];
        }

        private void Encipher(ref ulong left, ref ulong right)
        {
            for (int i = 0; i < Rounds; ++i)
            {
                left = left ^ _sPblocks.P[i];
                right = F(left) ^ right;

                swap(ref left, ref right);
            }

            swap(ref left, ref right);

            right = right ^ _sPblocks.P[Rounds];
            left = left ^ _sPblocks.P[Rounds + 1];
        }
    }
}
using System.Collections;
using UnityEngine;

namespace HeroFishing.Socket.Matchgame {
    public class UPDATEGAME_TOCLIENT : SocketContent {
        /// <summary>
        /// �C���}�lX��
        /// </summary>
        public double GameTime { get; private set; }
        /// <summary>
        /// �Ҧ����a���`��o�I��
        /// </summary>
        public long[] PlayerGainPoints { get; private set; } = new long[4];

        public UPDATEGAME_TOCLIENT() {
        }
    }
}
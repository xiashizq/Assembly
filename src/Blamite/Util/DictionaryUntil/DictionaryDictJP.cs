using System;
using System.Collections.Generic;

namespace Blamite.Util.DictionaryUntil
{
    internal class DictionaryDictJP : DictionaryDictBase
    {
        // 日语大字典
        protected override Dictionary<string, string> Dictionary => dictionaryParts;

        private static Dictionary<string, string> dictionaryParts = new Dictionary<string, string>
        {
            { "Object Type", "オブジェクトタイプ" },
            { "Biped", "バイパード" },
            { "Vehicle", "車両" },
            { "Weapon", "武器" },
            { "Equipment", "装備" },
            { "Terminal", "ターミナル" },
            { "Projectile", "発射物" },
            { "Scenery", "情景" },
            { "Machine", "機械" },
            { "Control", "コントロール" },
            { "Sound Scenery", "サウンド情景" },
            { "Crate", "木箱" },
            { "Creature", "クリーチャー" },
            { "Giant", "巨人" },
            { "Effect Scenery", "エフェクト情景" },
            { "Secondary Flags", "セカンダリーフラグ" },
            { "Does Not Affect Projectile Aiming", "発射物の照準に影響しない" },
            { "Flags", "フラグ" },
            { "Does Not Cast Shadow", "影を落とさない" },
            { "Search Cardinal Direction Lightmaps on Failure", "失敗時に基本方向のライトマップを検索" },
            { "Preserves Initial Damage Owner", "初期ダメージ所有者を保持" },
            { "Not A Pathfinding Obstacle", "経路探索の障害物ではない" },
            { "Extension Of Parent", "親の拡張" },
            { "Does Not Cause Collision Damage", "衝突ダメージを引き起こさない" },
            { "Early Mover", "先行移動者" }
        };
    }
}
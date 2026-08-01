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
            { "Early Mover", "先行移動者" },

            // ===== UI Menu =====
            { "FILE", "ファイル" },
            { "VIEW", "表示" },
            { "TOOLS", "ツール" },
            { "XBOX", "XBOX" },
            { "XBOX 360", "XBOX 360" },
            { "HELP", "ヘルプ" },
            { "Open File...", "ファイルを開く..." },
            { "Exit", "終了" },
            { "Start Page", "スタートページ" },
            { "Imgur History", "Imgur 履歴" },
            { "Halo 4 Voxel Converter", "Halo 4 ボクセル変換" },
            { "Map Compressor", "マップ圧縮" },
            { "Map Patcher", "マップパッチ" },
            { "Post Generator", "投稿ジェネレーター" },
            { "Plugin Generator", "プラグイン生成" },
            { "Plugin Converter", "プラグイン変換" },
            { "Group Poking", "グループポーク" },
            { "Tag Listings", "Tag 一覧" },
            { "Translation Setting", "翻訳設定" },
            { "GPT Setting", "GPT 設定" },
            { "Settings", "設定" },
            { "Take Screenshot", "スクリーンショット" },
            { "Stop", "停止" },
            { "Go", "再開" },
            { "Cold Reboot", "コールドリブート" },
            { "Soft Reboot", "ソフトリブート" },
            { "Title Reboot", "タイトルリブート" },
            { "Sync Time", "時刻同期" },
            { "Map Names", "マップ名" },
            { "Check for Updates", "更新を確認" },
            { "About Assembly", "Assembly について" },
            { "Temporarily halts the console.", "コンソールを一時停止します。" },
            { "Resumes a stopped console.", "停止中のコンソールを再開します。" },
            { "Fully reboots your console.", "コンソールを完全に再起動します。" },
            { "Reboots your console to your dashboard.", "ダッシュボードへ再起動します。" },
            { "Reboots the title currently running on your console.", "実行中のタイトルを再起動します。" },
            { "Sets the console time to the current system time.", "コンソール時刻をシステム時刻に合わせます。" }
        };
    }
}
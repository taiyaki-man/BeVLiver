using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VTuberChat
{
    // 起承転結のフェーズ
    public enum TalkPhase { Ki = 0, Sho = 1, Ten = 2, Ketsu = 3 }

    // ゲームバランス用の定数群（Inspectorで調整可能）
    [CreateAssetMenu(fileName = "GameBalance", menuName = "VTuberChat/Game Balance")]
    public class GameBalance : ScriptableObject
    {
        [Header("視聴者数の増減（ベース）")]
        public int baseStartViewers = 100;
        public int successDelta = 15;
        public int failDelta = -12;
        public int chainBonus = 8;                 // テーマやタグが続いた時のボーナス
        public float timeBonusPerSecond = 0.5f;    // 余り時間×係数

        [Header("自然増減（ドリフト）/ 秒")]
        public Vector2 driftPerSecond = new Vector2(-0.6f, 0.8f);

        [Header("判定しきい値")]
        [Range(0, 3)] public int minSharedTagsForSuccess = 1; // 共有タグがこの数以上で成功
    }

    // 成功/失敗時などの定型コメント集
    [CreateAssetMenu(fileName = "CommentSet", menuName = "VTuberChat/Comment Set")]
    public class CommentSet : ScriptableObject
    {
        [TextArea] public List<string> success = new();
        [TextArea] public List<string> fail = new();
        [TextArea] public List<string> generic = new();

        public string PickSuccess() => success.Count > 0 ? success[UnityEngine.Random.Range(0, success.Count)] : "ナイス！";
        public string PickFail() => fail.Count > 0 ? fail[UnityEngine.Random.Range(0, fail.Count)] : "？？？";
        public string PickGeneric() => generic.Count > 0 ? generic[UnityEngine.Random.Range(0, generic.Count)] : "w";
    }

    // 1枚のカード（話のタネ）
    [CreateAssetMenu(fileName = "TalkCard", menuName = "VTuberChat/Talk Card")]
    public class TalkCard : ScriptableObject
    {
        [Header("識別情報")]
        [Tooltip("一意なID（英数字）/ 外部参照やログで使います")]
        public string cardId = Guid.NewGuid().ToString("N").Substring(0, 8);

        [Header("表示")]
        [Tooltip("手札に表示する短い見出し（15文字程度）")]
        public string title;
        [TextArea] public string subtitle; // 説明文（任意）

        [Header("分類")]
        public TalkPhase phase = TalkPhase.Ki; // 起承転結
        [Tooltip("話題テーマ（自由記述）例: 泥棒/学校/家族 など")]
        public string theme;
        [Tooltip("整合判定に使うタグ。前カードとの共通数で成功判定")]
        public List<string> coherenceTags = new();

        [Header("スコア")]
        [Range(1, 5)] public int rarity = 1; // レア度
        public int baseScore = 10;           // カード固有の加点（任意）

        private void OnValidate()
        {
            // 余計な空白を除去、NULLを防止
            if (coherenceTags == null) coherenceTags = new List<string>();
            for (int i = 0; i < coherenceTags.Count; i++)
            {
                coherenceTags[i] = (coherenceTags[i] ?? string.Empty).Trim();
            }
            // タイトル未入力防止
            if (string.IsNullOrWhiteSpace(title)) title = name;
        }
    }

    // デッキ（今回のプレイで使うカードの束）
    [CreateAssetMenu(fileName = "TalkDeck", menuName = "VTuberChat/Talk Deck")]
    public class TalkDeck : ScriptableObject
    {
        [Tooltip("今回のプレイで使用するカード一覧（重複OK）")]
        public List<TalkCard> cards = new();

        public IReadOnlyList<TalkCard> GetByPhase(TalkPhase p)
            => cards.Where(c => c && c.phase == p).ToList();

        public bool ContainsId(string id) => cards.Any(c => c && c.cardId == id);

        private void OnValidate()
        {
            // Null混入や重複の軽い警告
            cards.RemoveAll(c => c == null);
        }
    }

    // 判定ロジック（最小実装）
    public static class CoherenceJudge
    {
        public struct Result
        {
            public bool success;
            public int deltaViewers;
            public int deltaScore;
            public int sharedTags;
            public bool chain;
        }

        public static Result Evaluate(
            TalkCard current,
            TalkCard previous,
            TalkPhase expectedPhase,
            GameBalance balance,
            float remainingSeconds)
        {
            var r = new Result();
            bool phaseOK = current.phase == expectedPhase;
            int shared = 0;
            bool chain = false;
            if (previous != null)
            {
                shared = CountSharedTags(previous.coherenceTags, current.coherenceTags);
                chain = !string.IsNullOrEmpty(previous.theme) && previous.theme == current.theme;
            }

            bool success = phaseOK && (shared >= balance.minSharedTagsForSuccess);
            int delta = success ? balance.successDelta : balance.failDelta;
            if (success)
            {
                delta += current.baseScore;
                if (chain) delta += balance.chainBonus;
                delta += Mathf.CeilToInt(remainingSeconds * balance.timeBonusPerSecond);
            }

            r.success = success;
            r.deltaViewers = delta;
            r.deltaScore = success ? current.baseScore : 0;
            r.sharedTags = shared;
            r.chain = chain;
            return r;
        }

        private static int CountSharedTags(List<string> a, List<string> b)
        {
            if (a == null || b == null) return 0;
            var set = new HashSet<string>(a.Where(s => !string.IsNullOrWhiteSpace(s)));
            int count = 0;
            foreach (var t in b)
            {
                if (!string.IsNullOrWhiteSpace(t) && set.Contains(t)) count++;
            }
            return count;
        }
    }

    // LiveシーンでSOを使う例（インスペクタで参照を差し込むだけ）
    public class DeckProvider : MonoBehaviour
    {
        [Header("参照（インスペクタで割当て）")]
        public TalkDeck deck;
        public GameBalance balance;
        public CommentSet comments;

        public TalkCard PickRandom(TalkPhase phase)
        {
            var list = deck?.GetByPhase(phase);
            if (list == null || list.Count == 0) return null;
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
    }
}

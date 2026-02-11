using LikeLion.LP0.Client.Core;
using LikeLion.LP0.Client.Core.GameScene;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace LikeLion.LP0.Client.UnityWorld.GameScene
{
    public class AIConsole : IAIConsole
    {
        private string _apiKey = "AIzaSyB5hcDm3HXlPc1i9MnvP4m0TW3iCT1lVuA";
        private string _url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key=";
        private readonly Core.ILogger _logger;

        [Serializable]
        public class GeminiResponse
        {
            public List<Candidate> candidates;
        }

        [Serializable]
        public class Candidate
        {
            public Content content;
            public string finishReason;
        }

        [Serializable]
        public class Content
        {
            public List<Part> parts;
            public string role;
        }

        [Serializable]
        public class Part
        {
            public string text;
        }

        [Serializable]
        public class AiResponse
        {
            public string thought;
            public int x;
            public int y;
        }

        public AIConsole(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<Tuple<int, int>> RequestStonePoint(int stoneType, int[][] array, CancellationToken token)
        {
            try
            {
                var whiteStones = new List<int[]>();
                var blackStones = new List<int[]>();

                for (var i = 0; i < array.Length; i++)
                {
                    for (var j = 0; j < array[i].Length; j++)
                    {
                        if (array[i][j] == StoneType.White)
                            whiteStones.Add(new int[2] { i, j });
                        if (array[i][j] == StoneType.Black)
                            blackStones.Add(new int[2] { i, j });
                    }
                }

                var move = await GetAiMoveAsync(15, stoneType, blackStones, whiteStones, 0, token);

                return new Tuple<int, int>(move.x, move.y);
            }
            catch (OperationCanceledException)
            {
                _logger.Info("AI request was canceled.");
                return default;
            }
            catch (Exception e)
            {
                _logger.Fatal(e.Message);
                return default;
            }
        }

        private async Task<AiResponse> GetAiMoveAsync(int size, int turn, List<int[]> blackStones, List<int[]> whiteStones,
            int difficulty, CancellationToken token)
        {
            var omokData = new
            {
                placed_stones = new { black = blackStones, white = whiteStones }
            };

            string omokJson = JsonConvert.SerializeObject(omokData);
            string prompt = $@"
## Role: Gomoku Expert (Fast Mode)
## Task: Return only the next move coordinate in JSON.
## Game Rule: {size}x{size}, (0,0) is bottom-left. Win at 5 in a row.
## Difficulty: {difficulty} (0:Defensive, 1:Aggressive, 2:Tactical/VCT)
## Input: Turn {turn}, State: {omokJson}

## Strict Constraint:
1. No thinking process.
2. No explanation.
3. No code blocks (```json).
4. Output ONLY: {{""x"": x, ""y"": y}}";

            var requestBody = new
            {
                contents = new[] {
            new { parts = new[] { new { text = prompt } } }
        }
            };

            string finalJson = JsonConvert.SerializeObject(requestBody);
            byte[] jsonToSend = Encoding.UTF8.GetBytes(finalJson);

            using (UnityWebRequest www = new UnityWebRequest(_url + _apiKey, "POST"))
            {
                www.uploadHandler = new UploadHandlerRaw(jsonToSend);
                www.downloadHandler = new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");

                var operation = www.SendWebRequest();

                while (!operation.isDone)
                {
                    if (token.IsCancellationRequested)
                    {
                        www.Abort();
                        token.ThrowIfCancellationRequested();
                    }
                    await Task.Yield();
                }

                if (www.result != UnityWebRequest.Result.Success)
                {
                    throw new Exception(www.error);
                }

                var root = JsonConvert.DeserializeObject<GeminiResponse>(www.downloadHandler.text);
                string aiText = root.candidates[0].content.parts[0].text;

                string cleanedJson = aiText.Replace("```json", "").Replace("```", "").Trim();
                cleanedJson = ExtractJson(cleanedJson);

                _logger.Info(omokJson);
                _logger.Info(cleanedJson);

                return JsonConvert.DeserializeObject<AiResponse>(cleanedJson);
            }
        }

        public static string ExtractJson(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            var match = Regex.Match(input, @"\{[\s\S]*\}", RegexOptions.Singleline);

            return match.Success ? match.Value : string.Empty;
        }
    }
}

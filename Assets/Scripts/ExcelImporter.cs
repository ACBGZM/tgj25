#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Data;
using ExcelDataReader;
using System.IO;
using DataDefinition;

public class ExcelDialogueImporter
{
    [MenuItem("Tools/Import NPC Dialogue Excel")]
    public static void Import()
    {
        string path = EditorUtility.OpenFilePanel("Select Excel", "", "xlsx");
        if (string.IsNullOrEmpty(path)) return;

        using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
        using (var reader = ExcelReaderFactory.CreateReader(stream))
        {
            var dataSet = reader.AsDataSet();

            foreach (System.Data.DataTable sheet in dataSet.Tables)
            {
                ImportNPCSheet(sheet);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static void ImportNPCSheet(DataTable table)
    {
        string npcName = table.TableName;

        var npcPool = ScriptableObject.CreateInstance<NPCTurnPoolSO>();
        var playerPool = ScriptableObject.CreateInstance<PlayerTurnPoolSO>();

        npcPool.npcTurns = new List<NPCTurnSO>();
        playerPool.playerTurns = new List<PlayerTurnSO>();

        int row = 3;

        // ---------- NPC TURNS ----------
        while (row < 19 && !IsRowEmpty(table, row))
        {
            string npcText = Cell(table, row, 2); // C
            if (string.IsNullOrEmpty(npcText))
            {
                row++;
                continue;
            }

            var npcTurn = ScriptableObject.CreateInstance<NPCTurnSO>();
            npcTurn.level = Convert.ToInt32(table.Rows[row][1]);
            npcTurn.text = npcText;
            npcTurn.playerChoices = new List<PlayerChoice>();

            string npcTurnDir = $"Assets/GameData/Conversation/{npcName}/NPCTurns/";
            Directory.CreateDirectory(npcTurnDir);

            for (int i = 0; i < 3; i++)
            {
                int baseCol = 3 + i * 3; // D / G / J

                string option = Cell(table, row, baseCol);
                string response = Cell(table, row, baseCol + 1);
                string deltaStr = Cell(table, row, baseCol + 2);

                if (string.IsNullOrEmpty(option)) continue;

                int delta = 0;
                int.TryParse(deltaStr, out delta);

                npcTurn.playerChoices.Add(new PlayerChoice
                {
                    textPrompt = option,
                    textLine = option,
                    npcResponse = response,
                });
            }

            npcPool.npcTurns.Add(npcTurn);
            row++;

            AssetDatabase.CreateAsset(npcTurn, $"{npcTurnDir}/NPCTurn_{row}.asset");
        }

        string playerTurnDir = $"Assets/GameData/Conversation/{npcName}/PlayerTurns/";
        Directory.CreateDirectory(playerTurnDir);

        row = 22;
        // ---------- PLAYER TURNS ----------
        while (row < 39)
        {
            string topic = Cell(table, row, 2); // C
            if (string.IsNullOrEmpty(topic))
            {
                row++;
                continue;
            }

            string playerLine = Cell(table, row, 3); // D
            string npcResponse = Cell(table, row, 4); // E
            int delta = 0;
            int.TryParse(Cell(table, row, 5), out delta); // F

            var playerTurn = ScriptableObject.CreateInstance<PlayerTurnSO>();
            playerTurn.level = Convert.ToInt32(table.Rows[row][1]);
            playerTurn.playerTopic = new PlayerChoice
            {
                textPrompt = topic,
                textLine = playerLine,
                npcResponse = npcResponse,
            };

            playerPool.playerTurns.Add(playerTurn);
            row++;

            AssetDatabase.CreateAsset(playerTurn, $"{playerTurnDir}/NPCTurn_{row}.asset");
        }

        string dir = $"Assets/GameData/Conversation/{npcName}";
        Directory.CreateDirectory(dir);

        AssetDatabase.CreateAsset(npcPool, $"{dir}/NPCTurnPool.asset");
        AssetDatabase.CreateAsset(playerPool, $"{dir}/PlayerTurnPool.asset");
    }

    static string Cell(System.Data.DataTable table, int row, int col)
    {
        if (row >= table.Rows.Count || col >= table.Columns.Count)
            return string.Empty;

        return table.Rows[row][col]?.ToString().Trim();
    }

    static bool IsRowEmpty(System.Data.DataTable table, int row)
    {
        return string.IsNullOrEmpty(Cell(table, row, 2)); // C 列
    }
}
#endif
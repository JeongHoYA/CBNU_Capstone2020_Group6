using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
//using System.Diagnostics;

public  static class SaveSystem
{
    public static void SaveWorld (WorldData world)
    {
        string savePath = World.Instance.mapFileLocation;

        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        Debug.Log("Saving " + world.worldName);

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(savePath + "world.world", FileMode.Create);

        formatter.Serialize(stream, world);
        stream.Close();

        Thread thread = new Thread(() => SaveChunks(world));
        thread.Start();
    }                                                           // 월드를 저장하는 함수

    public static void SaveChunks(WorldData world)
    {
        List<ChunkData> chunks = new List<ChunkData>(world.modifiedChunks);
        world.modifiedChunks.Clear();

        int count = 0;
        foreach (ChunkData chunk in chunks)
        {
            SaveSystem.SaveChunk(chunk, world.worldName);
            count++;
        }
        Debug.Log(count + " Chunks saved");
    }                                                           // Thread를 이용해 월드에 청크를 저장하는 함수

    public static void SaveChunk(ChunkData chunk, string worldName)
    {
        string chunkName = chunk.position.x + "-" + chunk.position.y;

        string savePath = World.Instance.mapFileLocation + worldName + " ChunkData/";

        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(savePath + chunkName + ".chunk", FileMode.Create);

        formatter.Serialize(stream, chunk);
        stream.Close();
    }                                          // worldName 아래 chunk를 저장하는 함수
        


    public static WorldData LoadWorld(string worldName)
    {
        string loadPath = World.Instance.mapFileLocation;

        if(File.Exists(loadPath + "world.world"))
        {
            Debug.Log(worldName + " found. Loading from Save.");

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(loadPath + "world.world", FileMode.Open);

            WorldData world = formatter.Deserialize(stream) as WorldData;
            stream.Close();

            return new WorldData(world);
        }
        else
        {
            Debug.Log(worldName + "Not found, Creating new world.");

            WorldData world = new WorldData(worldName);
            SaveWorld(world);

            return world;
        }
    }                                                      // worldName을 불러오는 함수

    public static ChunkData LoadChunk(string worldName, Vector2Int position)
    {
        string chunkName = position.x + "-" + position.y;

        string loadPath = World.Instance.mapFileLocation + worldName + " ChunkData/" + chunkName + ".chunk";

        if (File.Exists(loadPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(loadPath, FileMode.Open);

            ChunkData chunkData = formatter.Deserialize(stream) as ChunkData;
            stream.Close();

            return chunkData;
        }
        else
            return null;
    }                                 // worldName의 월드 내 position에 존재하는 Chunkdata를 불러오는 함수
}

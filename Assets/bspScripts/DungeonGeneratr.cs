using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class DungeonGeneratr
{
    
    List<RoomNode> allNodesCollection = new List<RoomNode>();
    private int dungeonWidth;
    private int dungeonLength;

    public DungeonGeneratr(int dungeonWidth, int dungeonLength)
    {
        this.dungeonWidth = dungeonWidth;
        this.dungeonLength = dungeonLength;
    }

    public List<Node> CalculateDungeon(int maxIterations, int roomWidthMin, int roomLengthMin, float roomBottomCornerModifier, float roomTopCornerModifier, int roomOffset, int corridorWidth)
    {
        //create a binary space partition
        BinarySpacePartitioner bsp = new BinarySpacePartitioner(dungeonWidth, dungeonLength); 
        allNodesCollection = bsp.PrepareNodesCollection(maxIterations, roomWidthMin, roomLengthMin);
        List<Node> roomSpaces = StructureHelper.TraverseGraphToExtractLowestLeafes(bsp.RootNode);
        
        RoomGenerator roomGenerator = new RoomGenerator(maxIterations,roomLengthMin,roomWidthMin);
        List<RoomNode> roomList = roomGenerator.GenerateRoomsInGivenSpaces(roomSpaces, roomBottomCornerModifier, roomTopCornerModifier, roomOffset);

        CorridorsGenerator corridorsGenerator = new CorridorsGenerator();
        var corridorList = corridorsGenerator.CreateCorridor(allNodesCollection, corridorWidth);


        return new List<Node>(roomList).Concat(corridorList).ToList();

    }

    internal object CalculateRooms(int maxIterations, int roomWidthMin, int roomLengthMin, float roomBottomCornerModifier, float roomTopCornerModifier, int roomOffset, int corridorWidth)
    {
        throw new NotImplementedException();
    }
}
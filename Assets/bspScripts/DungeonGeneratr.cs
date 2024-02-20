﻿using System;
using System.Collections.Generic;
using UnityEngine;
public class DungeonGeneratr
{
    
    List<RoomNode> allSpaceNodes = new List<RoomNode>();
    private int dungeonWidth;
    private int dungeonLength;

    public DungeonGeneratr(int dungeonWidth, int dungeonLength)
    {
        this.dungeonWidth = dungeonWidth;
        this.dungeonLength = dungeonLength;
    }

    public List<Node> CalculateRooms(int maxIterations, int roomWidthMin, int roomLengthMin)
    {
        //create a binary space partition
        BinarySpacePartitioner bsp = new BinarySpacePartitioner(dungeonWidth, dungeonLength); 
        allSpaceNodes = bsp.PrepareNodesCollection(maxIterations, roomWidthMin, roomLengthMin);
        List<Node> roomSpaces = StructureHelper.TraverseGraphToExtractLowestLeafes(bsp.RootNode);
        
        RoomGenerator roomGenerator = new RoomGenerator(maxIterations,roomLengthMin,roomWidthMin);
        List<RoomNode> roomList = roomGenerator.GenerateRoomsInGivenSpaces(roomSpaces);
        
        return new List<Node>(roomList);

    }
}
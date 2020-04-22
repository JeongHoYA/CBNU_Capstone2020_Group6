using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 // 박스를 월드에 스폰시켜주는 스크립트
public class World : MonoBehaviour
{
    public Material material; // 청크들에게 공통적으로 사용될 머터리얼(텍스처 아틀라스 사용)
    public BlockType[] blockTypes;

}


 // 박스의 타입들을 모아놓은 클래스
[System.Serializable]
public class BlockType
{
    public string blockName;
    public bool isSolid;
    public byte TextureID;
}

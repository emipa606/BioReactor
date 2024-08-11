using UnityEngine;
using Verse;

namespace BioReactor;

public class BioReactorDef : ThingDef
{
    /// <summary>
    ///     수용 생명체 크기 최대 한도
    /// </summary>
    public float bodySizeMax;

    /// <summary>
    ///     수용 생명체 크기 최소 한도
    /// </summary>
    public float bodySizeMin;

    /// <summary>
    ///     캐릭터 드로우 좌표. 리액터의 실좌표 중심을 기준으로 드로우.
    /// </summary>
    public Vector3 innerDrawOffset;

    /// <summary>
    ///     리액터 용액 드로우 중심 좌표. 리액터 실 좌표 중심을 기준으로 드로우
    /// </summary>
    public Vector3 waterDrawCenter;

    /// <summary>
    ///     리액터 용액 그래픽 넓이
    /// </summary>
    public Vector2 waterDrawSize;
}
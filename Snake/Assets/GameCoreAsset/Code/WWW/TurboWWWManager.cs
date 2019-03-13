using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurboWWWManager : Singleton<TurboWWWManager>
{
    /// <summary>
    /// Token, got in autorization 
    /// </summary>
    public string AccessToken = "";
    /// <summary>
    /// Date when access token expires
    /// </summary>
    private DateTime AccessTokenExpire; 
}

/// <summary>
/// Request types
/// </summary>
public enum RequestType
{
    /// <summary>
    /// Usual GET
    /// </summary>
    GET,
    /// <summary>
    /// Usual POST
    /// </summary>
    POST,
    /// <summary>
    /// Usual PUT
    /// </summary>
    PUT,
    /// <summary>
    /// Usual DELETE
    /// </summary>
    DELETE,
    /// <summary>
    /// Usual NONE
    /// </summary>
    NONE,
    /// <summary>
    /// Usual GET_TEXTURE
    /// </summary>
    GET_TEXTURE,
}
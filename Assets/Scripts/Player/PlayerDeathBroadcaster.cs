using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家死亡事件广播器，用于通知所有注册的敌人和监听器
/// </summary>
public static class PlayerDeathBroadcaster
{
    private static readonly List<Enemy> baseEnemies = new();
    private static readonly List<IPlayerDeathListener> interfaceListeners = new();

    public static void Register(Enemy enemy)
    {
        if (enemy == null) return;
        
        if (!baseEnemies.Contains(enemy))
            baseEnemies.Add(enemy);
    }

    public static void Unregister(Enemy enemy)
    {
        if (enemy == null) return;
        
        baseEnemies.Remove(enemy);
    }

    public static void Register(IPlayerDeathListener listener)
    {
        if (listener == null) return;
        
        if (!interfaceListeners.Contains(listener))
            interfaceListeners.Add(listener);
    }

    public static void Unregister(IPlayerDeathListener listener)
    {
        if (listener == null) return;
        
        interfaceListeners.Remove(listener);
    }

    public static void Broadcast()
    {
        // 使用ToArray()创建副本，防止在遍历时集合被修改
        foreach (var enemy in baseEnemies.ToArray())
        {
            try
            {
                enemy?.OnPlayerDeath();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"执行敌人OnPlayerDeath时出错: {ex}");
            }
        }

        // 使用ToArray()创建副本，防止在遍历时集合被修改
        foreach (var listener in interfaceListeners.ToArray())
        {
            try
            {
                listener?.OnPlayerDeath();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"执行监听器OnPlayerDeath时出错: {ex}");
            }
        }
    }
}
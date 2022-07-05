using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedRandom : MonoBehaviour
{
    static int seed;
    static int mod = 1000000007;
    static long cur;
    public static void SetSeed(int newSeed)
    {
        seed = newSeed;
        cur = seed % mod;
        cur += 17;
        cur *= (cur + 13);
        cur += 1003 * seed;
        cur %= mod;
    }

    public static int Get(int x, int y)
    {
        long ret = x * x + 4 * y + seed + 997;
        ret %= mod;
        ret *= x ^ seed + y ^ seed + 7;
        ret += 3 * seed + 17 * y + seed * seed + 10007;
        ret %= mod;

        return (int)ret;
    }
}

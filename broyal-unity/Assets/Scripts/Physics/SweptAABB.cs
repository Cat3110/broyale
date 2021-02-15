using Unity.Mathematics;
using UnityEngine;

public static class SweptAABB
{
    public static Box GetBroadPhaseBox(Box b)
    {
        var broadPhaseBox = new Box(0.0f, 0.0f, 0.0f, 0.0f)
        {
            x = b.vx > 0 ? b.x : b.x + b.vx,
            y = b.vy > 0 ? b.y : b.y + b.vy,
            w = b.vx > 0 ? b.w + b.vx : b.w - b.vx,
            h = b.vy > 0 ? b.h + b.vy : b.h - b.vy
        };

        return broadPhaseBox;
    }
    public static bool AABBCheck(Box b1, Box b2) 
    { 
        return !(b1.x + b1.w < b2.x || b1.x > b2.x + b2.w || b1.y + b1.h < b2.y || b1.y > b2.y + b2.h); 
    }
    
    public static  bool SweepBoxBox( Box a, Box b, float2 a_min, float2 a_max, float2 b_min, float2 b_max, float2 v, out float2 outVel, out float2 hitNormal )
    {
        //Initialise out info
        outVel = v;
        hitNormal = Vector2.zero;

        // Return early if a & b are already overlapping
        if( AABBCheck(a, b) ) return false;

        // Treat b as stationary, so invert v to get relative velocity
        // v = -v;

        float hitTime = 0.0f;
        float outTime = 1.0f;
        Vector2 overlapTime = Vector2.zero;

        // X axis overlap
        if( v.x < 0 )
        {
            if( b_max.x < a_min.x ) return false;
            if( b_max.x > a_min.x ) outTime = Mathf.Min( (a_min.x - b_max.x) / v.x, outTime );

            if( a_max.x < b_min.x )
            {
                overlapTime.x = (a_max.x - b_min.x) / v.x;
                hitTime = Mathf.Max(overlapTime.x, hitTime);
            }
        }
        else if( v.x > 0 )
        {
            if( b_min.x > a_max.x ) return false;
            if( a_max.x > b_min.x ) outTime = Mathf.Min( (a_max.x - b_min.x) / v.x, outTime );

            if( b_max.x < a_min.x )
            {
                overlapTime.x = (a_min.x - b_max.x) / v.x;
                hitTime = Mathf.Max(overlapTime.x, hitTime);
            }
        }

        if( hitTime > outTime ) return false;

        //=================================

        // Y axis overlap
        if( v.y < 0 )
        {
            if( b_max.y < a_min.y ) return false;
            if( b_max.y > a_min.y ) outTime = Mathf.Min( (a_min.y - b_max.y) / v.y, outTime );

            if( a_max.y < b_min.y )
            {
                overlapTime.y = (a_max.y - b_min.y) / v.y;
                hitTime = Mathf.Max(overlapTime.y, hitTime);
            }           
        }
        else if( v.y > 0 )
        {
            if( b_min.y > a_max.y ) return false;
            if( a_max.y > b_min.y ) outTime = Mathf.Min( (a_max.y - b_min.y) / v.y, outTime );

            if( b_max.y < a_min.y )
            {
                overlapTime.y = (a_min.y - b_max.y) / v.y;
                hitTime = Mathf.Max(overlapTime.y, hitTime);
            }
        }

        if( hitTime > outTime ) return false;

        // Scale resulting velocity by normalized hit time
        outVel = -v * hitTime;

        // Hit normal is along axis with the highest overlap time
        if( overlapTime.x > overlapTime.y )
        {
            hitNormal = new Vector2(Mathf.Sign(v.x), 0);
        }
        else
        {
            hitNormal = new Vector2(0, Mathf.Sign(v.y));
        }

        return true;
    }
    
    public static bool CheckAABB(Box b1, Box b2, ref float moveX, ref float moveY)
    {
        moveX = 0.0f;
        moveY = 0.0f;

        float l = b2.x - (b1.x + b1.w);
        float r = (b2.x + b2.w) - b1.x;
        float t = b2.y - (b1.y + b1.h);
        float b = (b2.y + b2.h) - b1.y;

        if (l > 0 || r < 0 || t > 0 || b < 0)
            return false;

        // get the shortest collision interval, to add quickly 
        moveX = math.abs(l) < r ? l : r;
        moveY = math.abs(t) < b ? t : b;

        // get the direction with the shortest collision range
        if (math.abs(moveX) < math.abs(moveY))
        {
            moveY = 0.0f;
        }
        else
        {
            moveX = 0.0f;
        }

        return true;
    }

    public static float Swept(Box b1, Box b2, ref float normalx, ref float normaly)
    {
        float xInvEntry, yInvEntry;
        float xInvExit, yInvExit;

        // find the distance between the objects on the near and far sides for both x and y
        if (b1.vx > 0.0f)
        {
            xInvEntry = b2.x - (b1.x + b1.w);
            xInvExit = (b2.x + b2.w) - b1.x;
        }
        else
        {
            xInvEntry = (b2.x + b2.w) - b1.x;
            xInvExit = b2.x - (b1.x + b1.w);
        }

        if (b1.vy > 0.0f)
        {
            yInvEntry = b2.y - (b1.y + b1.h);
            yInvExit = (b2.y + b2.h) - b1.y;
        }
        else
        {
            yInvEntry = (b2.y + b2.h) - b1.y;
            yInvExit = b2.y - (b1.y + b1.h);
        }

        // find time of collision and time of leaving for each axis (if statement is to prevent divide by zero)
        float xEntry, yEntry;
        float xExit, yExit;

        if (b1.vx == 0.0f)
        {
            xEntry = -float.PositiveInfinity;
            xExit = float.PositiveInfinity;
        }
        else
        {
            xEntry = xInvEntry / b1.vx;
            xExit = xInvExit / b1.vx;
        }

        if (b1.vy == 0.0f)
        {
            yEntry = -float.PositiveInfinity;
            yExit = float.PositiveInfinity;
        }
        else
        {
            yEntry = yInvEntry / b1.vy;
            yExit = yInvExit / b1.vy;
        }

        // find the earliest/latest times of collision
        float entryTime = math.max(xEntry, yEntry);
        float exitTime = math.min(xExit, yExit);

        // if there was no collision
        if (entryTime > exitTime || xEntry < 0.0f && yEntry < 0.0f || xEntry > 1.0f || yEntry > 1.0f)
        {
            normalx = 0.0f;
            normaly = 0.0f;
            return 1.0f;
        }
        else // if there was a collision
        {
            // calculate normal of collided surface
            if (xEntry > yEntry && (b1.y + b1.h + 1.0f != b2.y ))
            {
                if (xInvEntry < 0.0f)
                {
                    normalx = 1.0f;
                    normaly = 0.0f;
                }
                else
                {
                    normalx = -1.0f;
                    normaly = 0.0f;
                }
            }
            else
            {
                if (yInvEntry < 0.0f)
                {
                    normalx = 0.0f;
                    normaly = 1.0f;
                }
                else
                {
                    normalx = 0.0f;
                    normaly = -1.0f;
                }
            }

            // return the time of collision
            return entryTime;
        }
    }
}
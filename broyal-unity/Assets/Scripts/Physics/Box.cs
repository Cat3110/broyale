// describes an axis-aligned rectangle with a velocity 
public struct Box
{
    // position of top-left corner 
    public float x, y;
        
    // dimensions 
    public float w, h;
        
    // velocity 
    public float vx, vy;

    public Box(float x, float y, float w, float h, float vx, float vy)
    {
        this.x = x;
        this.y = y;
        this.w = w;
        this.h = h;
        this.vx = vx;
        this.vy = vy;
    }

    public Box(float x, float y, float w, float h)
    {
        this.x = x;
        this.y = y;
        this.w = w;
        this.h = h;
        vx = 0.0f;
        vy = 0.0f;
    }
}

public static class BoxExt
{
    public static Box ToBox(this ColliderData collider)
    {
        return new Box(collider.Position.x - collider.Size.x * 0.5f, collider.Position.z - collider.Size.z * 0.5f,
            collider.Size.x, collider.Size.z);
    }
}
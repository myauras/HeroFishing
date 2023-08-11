package collision

import (
    "github.com/scozirge/2dcollision/collider"
)


//計算兩圓是否有碰撞到
func CirclesColliding(c1, c2 collider.Circle) bool {
	//這裡不使用開方跟指數運算是優化效能的考量
    distanceSquared := (c1.X - c2.X)*(c1.X - c2.X) + (c1.Y - c2.Y)*(c1.Y - c2.Y)//計算兩圓心距離平方
    sumOfRadius := c1.Radius + c2.Radius//計算兩圓半徑和
    return distanceSquared <= sumOfRadius*sumOfRadius//如果兩圓心距離平方小於等於兩圓半徑和的平方就是有兩圓有碰撞到
}


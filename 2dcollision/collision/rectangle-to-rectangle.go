package collision

import (
	"github.com/scozirge/2dcollision/collider"
)



// 判斷對齊兩軸的矩形間是否有碰撞
func AxisAligned_RectangleToRectangleColliding(r1, r2 collider.Rectangle1) bool {
	return r1.X <= r2.X+r2.Width &&
		r1.X+r1.Width >= r2.X &&
		r1.Y <= r2.Y+r2.Height &&
		r1.Y+r1.Height >= r2.Y
}

// 判斷兩矩形間是否有碰撞(使用分離軸定理 Separating Axis Theorem, SAT)
func RectangleToRectangleColliding(r1, r2 collider.Rectangle2) bool {
	// 矩形沒有對齊兩軸(旋轉的矩形)，可用「分離軸定理」（Separating Axis Theorem, SAT）來判斷兩個是否有碰撞。
	// 分離軸定理就是：如果能找到任一軸，使得兩個多邊形在這個軸上的投影沒有重疊，那麼這兩個多邊形就不可能碰撞。


	// 取得兩個矩形對應邊的法向量，因為矩形的邊彼此垂直(向量正交)，因此對於矩形，只需要計算一邊的向量，就可以得到相鄰邊的法向量
	axes := []collider.Vector2{
		{r1.P1.X - r1.P2.X, r1.P1.Y - r1.P2.Y},
		{r1.P1.X - r1.P4.X, r1.P1.Y - r1.P4.Y},
		{r2.P1.X - r2.P2.X, r2.P1.Y - r2.P2.Y},
		{r2.P1.X - r2.P4.X, r2.P1.Y - r2.P4.Y},
	}

	//這裡是簡化算法，只計算矩形的頂點與軸向量的點積(投影長度而不是絕對長度)，因為SAT中只關心是否有重疊就好

	for _, axis := range axes {
		if !overlap(project(r1, axis), project(r2, axis)) {
			return false
		}
	}

	return true
}

// 將矩形投影到軸上
func project(rect collider.Rectangle2, axis collider.Vector2) (float64, float64) {
	dots := []float64{
		dot(axis, rect.P1),
		dot(axis, rect.P2),
		dot(axis, rect.P3),
		dot(axis, rect.P4),
	}
	
	min := dots[0]
	max := dots[0]
	for _, d := range dots[1:] {
		if d < min {
			min = d
		}
		if d > max {
			max = d
		}
	}

	return min, max
}

// 檢查兩個投影是否有重疊
func overlap(minMax1, minMax2 (float64, float64)) bool {
	return minMax1[1] >= minMax2[0] && minMax2[1] >= minMax1[0]
}

// 計算兩個向量的點積
func dot(v1, v2 collider.Vector2) float64 {
	return v1.X*v2.X + v1.Y*v2.Y
}

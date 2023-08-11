package main

import (
    "fmt"
    "github.com/scozirge/2dcollision/collider"
	"github.com/scozirge/2dcollision/collision"
)

func main() {
	c1:=collider.Circle{
		X:2,
		Y:3,
		Radius:1,
	}

	c2:=collider.Circle{
		X:1,
		Y:1,
		Radius:2,
	}

	c1c2collision:=collision.CirclesColliding(c1,c2)
	fmt.Printf("c1跟c2碰撞檢測結果: %v\n", c1c2collision)
}
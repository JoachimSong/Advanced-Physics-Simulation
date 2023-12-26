# Advanced-Physics-Simulation
![Game Screenshot](https://songjc-portfolio-1323252154.cos.ap-shanghai.myqcloud.com/advanced_physics_simulation_1.jpg)
## Overview

Simple Hair and Cloth Simulation made with Unity.

## Table of Contents
- [Project Description](#project-description)
- [Features](#features)
- [Implementation Details](#implementation-details)
- [Installation](#installation)
- [Controls](#controls)
- [Screenshots](#screenshots)
- [Usage License](#usage-license)

## Project Description

Project Objectives:
1. Understand how physical phenomena are modeled in games
2. Understand several methods of solving ordinary differential equations
3. Learn the basic spring model implementation

## Features

### Hair Simulation
- Simple interaction: the player can adjust hair weight, damping factor, length and other parameters through UI

### Cloth Simulation
- Simple interaction: the player can use the ball to collide with the cloth

## Implementation Details

### Hair Simulation
1. Implementing dynamic effects using the Verlet integration method.
2. Enforcing spring constraints on the hair strands.
3. Introducing collision constraints between the hair strands and the spherical head model.
4. Incorporating simple user interactions.
5. Utilizing a UI to adjust parameters such as hair weight, damping coefficient, and length.
6. Exploring optional enhancements, such as advanced rendering effects.

### Cloth Simulation
1. Utilizing a grid-based mass-spring system for cloth modeling.
2. Calculating external forces acting on each cloth vertex, focusing on gravitational forces.
3. Computing structural, shear, and flexion forces within the cloth through spring constraints between different vertices.
4. Implementing dynamic cloth effects using the Semi-implicit Euler (Symplectic Euler) method.
5. Exploring optional enhancements, such as cloth-sphere collisions.
6. Implementing cloth simulation using the implicit Euler method.


## Installation

Download and unzip the **Advanced Physics_Build.zip**, then open the **Advanced Physics.exe** to launch the program.

## Controls

- **&larr;/&rarr;/&uarr;/&darr;/J/K:** Move the ball
- **ESC:** Quit

## Screenshots

![Screenshot 1](https://songjc-portfolio-1323252154.cos.ap-shanghai.myqcloud.com/advanced_physics_simulation_2.png)

![Screenshot 2](https://songjc-portfolio-1323252154.cos.ap-shanghai.myqcloud.com/advanced_physics_simulation_3.png)


## Usage License

The use of this project is subject to the following constraints:

1. For Academic Use Only: This project is intended solely for academic assignments and research purposes at Shanghai Jiao Tong University. Its use for commercial purposes or any unauthorized use is strictly prohibited.

2. No Redistribution of University Information: The project contains information specific to Shanghai Jiao Tong University, and without permission, it may not be redistributed in any form.

3. All Rights Reserved: The author reserves all rights to this project and its related content.

# 自动泊车项目

## 项目结构

```
├─.vscode
├─Examples   //我们的Unity项目文件夹 包含所有场景与脚本      
│  ├─MAPD    
│  │  ├─Demo
│  │  ├─Prefabs
│  │  │  └─Materials
│  │  │      └─Textures
│  │  ├─Scene    //Unity场景
│  │  ├─Script   //Unity脚本
│  │  │  ├─Communication   //与ROS通信相关的脚本
│  │  │  ├─Exp_Agent       //学术实验有关脚本
│  │  │  ├─Exp_Car         //学术实验有关脚本
│  │  │  └─Parking         //泊车与行车有关脚本
│  │  └─TFModels
├─RosMessages    //编译后的ROS消息，方便Unity脚本调用
│  ├─Geometry   
│  │  └─msg
│  └─Sensor
│      ├─msg
│      └─srv
├─Scripts   //GridSensor2 项目脚本
   └─Sensors 
       ├─Grid
       │  ├─Basic
       │  │  ├─Debug
       │  │  ├─Detection
       │  │  ├─Encoding
       │  │  │  └─Modifiers
       │  │  └─Sensor
       │  └─GameObject
       │      ├─Detection
       │      │  └─Constraints
       │      └─Sensor
       └─Util
```



## Unity-ROS通信

[参考链接](https://blog.csdn.net/gls_nuaa/article/details/128035034)

### 1.Unity配置

#### 安装package

打开项目，安装**ros-tcp-connector**与**visualizations**两个packages。方法如下：

1. Unity编辑器菜单栏Window -> Package Manager

2. 在Package Manager中，点击左上角加号，选择Add package from git url

3. ```
   https://github.com/Unity-Technologies/ROS-TCP-Connector.git?path=/com.unity.robotics.ros-tcp-connector
   https://github.com/Unity-Technologies/ROS-TCP-Connector.git?path=/com.unity.robotics.visualizations
   ```

   

导入成功后Package Manager中会显示这两个包。

若因为网络问题无法使用url导入时，可将上述git项目clone到本地，然后导入方式选择Add package from disk，导入上面两个com文件夹下的package.json即可。

#### 配置网络地址

上一步完成后Unity菜单栏中会出现Robotics选项，点开选择ROS Settings，修改ROS IP Address为ROS环境IPV4地址（192.162.xxx.xxx），端口一般保持10000不变。IPV4地址可通在Ubuntu系统下使用ifconfig命令查询。

#### 编译Msg or Srv文件

Robotics选项下的Generate ROS Messages功能可以将ROS支持的msg和src文件转换为Unity支持的C#文件，将msg文件所在的（文件夹）地址传入，展开下方的折叠菜单点击Build x msgs或Build 

x srv,即可将编译后的文件保存至RosMessages文件夹下

### 2.ROS配置

#### 下载ROS-TCP-Endpoint放在工作空间编译

```
下载地址：https://github.com/Unity-Technologies/ROS-TCP-Endpoint.git
```

#### 启动ROS-TCP-Endpoint

```
source devel/setup.bash
roslaunch ros_tcp_endpoint endpoint.launch tcp_ip:=192.168.xxx.xxx tcp_port:=10000
```

### 3.通信

#### Unity发布ROS订阅

在ParkCar scene中选中ParkDriveAgent后，在其Inspector中勾选ROS Publisher（若没有则手动从Assets/Examples/MAPD/Script/Communication下将该脚本拖入Inspector），启动Unity。连接成功时，Unity场景左上角会显示蓝色双向箭头，Unity向/cmd_vel话题发布运动命令，ROS端任意差速控制小车启动后均可订阅该命令，进行圆周运动。

#### Unity订阅ROS发布

首先在ROS端启动能够发布/scan话题雷达信息的节点（如启动ROS motion planning项目），然后在ParkCar scene中选中ParkDriveAgent后，在其Inspector中勾选ROS Subscriber，启动Unity，连接成功后，Unity会使用Debug.Log打印雷达信息。

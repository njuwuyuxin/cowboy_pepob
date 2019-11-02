## 开发环境
Unity 2019.2.5f1 Personal

## 关于charactor controller脚本使用方法
该脚本主要通过射线检测的方式控制人物移动和碰撞，不使用Unity原生刚体。
### 使用方法

1. 使用时需要将character controller脚本挂载到目标角色上。
2. 由于使用charactor controller控制人物移动，因此角色刚体组件需要选择为Kinematic而不是默认的动态（否则会按照原生物理引擎处理）
3. 脚本中可以设置哪些图层为不可穿越平台（平台遮罩），哪些图层为触发器类型，哪些图层为可穿越平台（OneWayPlatform），均为多选
4. 设置好上述类型后，在新添加地图、平台之类Sprite时，记得设置图层为对应类型
5. charactor controller对外实际主要暴露一个Move接口。因此为了控制角色移动，还需要绑定另一个负责移动的脚本来调用Move接口，同时绑定碰撞处理事件，具体可以仿照已有的主角的Move脚本

### 注意事项
- 需要将挂载该脚本的角色图层设为Player（Enemy），默认会发生碰撞的图层为Default，因此不修改为Player会发生角色自身碰撞问题。
- 设置为可穿越平台的物体，其碰撞体组件需要设置为边缘碰撞体（EdgeCollider2D），其厚度为0 否则会出现卡在碰撞体中的现象
- 设置为边缘碰撞体后如果发生人物陷入平台中的现象，可以适当调节碰撞体的偏移量
- 脚本中各项参数含义参照脚本内注释
- 鉴于除了主角之外，敌人也使用了Character Controller脚本，为了让主角和敌人碰撞时能够触发Trigger事件，主角单方面设置Trigger Mask是不够的，还需要把敌人身上的Trigger Mask设置为主角图层。双向均设置完毕后，即可正常触发Trigger事件。

## 关于A*寻路插件的使用方法
暂时只使用插件模式，后期可以改成脚本模式。
### 使用方法

1. 首先在场景中创建寻路区域，例如：创建名为A*的空对象，然后增加PathFinder的组件，使用Graphs创建 GridGraph，勾选2D，2D Physics，然后选择Obstacle图层（可多选），最后Scan 生成寻路地图
2. 在敌人对象上增加AI Path组件和AI Destination 组件，选定SnowMan作为Target

## 电梯物体(Elevator)使用注意事项
### 各项参数释义
- Elevator Direction 用于控制电梯初始移动方向：1=向上 -1=向下 2=向右 -2=向左
- Move Distance 控制该电梯启动一次移动的距离
- Elevator Speed 电梯移动速度
- isAuto 勾选为自动移动，可用作移动平台。非勾选为正常电梯，主角需按E启动
- Ray Count 由于电梯物体通过向上方发射射线来检测主角是否站在上面，射线在物体上横向均匀分布，数量由该参数控制。如果太小，可能出现射线间隙过大检测不到主角。如果电梯宽度较高或出现检测失灵情况，应适量调高射线数以提高精度

## 场景切换组件使用方法
场景切换主要通过Portal物体实现，Portal可以放置在需要切换场景的位置，可以自定义大小（注意保持Collider大小一致） Portal为一个Trigger，当Portal检测到主角进入触发区域后，即会自动加载新场景。
### 各项参数释义
- Scene Name 当主角进入该Portal时，要加载的新场景名称
- Character Position 当主角进入新的场景时，出现在新场景中的位置（从不同入口进入同一张地图，加载新场景时，主角在新场景的初始位置也会不同）
### 注意事项
- 当需要加载新场景时，注意要把新场景加入到Build Settings当中。位置：文件->Build Settings->添加已打开的场景。 否则会报错：Scene 'xxx' couldn't be loaded because it has not been added to the build settings or the AssetBundle has not been loaded.

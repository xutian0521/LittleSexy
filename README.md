# LittleSexy

#### 前端页面
1. element-ui
2. vue.js
3. axios
4. CDN
5. SEO
6. Chimee --H5视频播放器组件
7. hexo-theme 前端页面主题
#### 前端页面Api

1. mysql 用户信息关系数据库，所有电影列表信息，瀑布流图片用mongodb
2. oss
3. DI
4. restful api
5. git flow
6. redis (CSRedis)-做缓存, 点赞, 热点评论排序
7. mongodb -社区发帖 -tab页面信息
8. CDN(静态文件图片等使用 图片接口 或者阿里云oss)

#### 后台管理系统前端
1. ant design
2. react.js
3. 

#### 后台管理系统Api
1. mysql
2. EF Core

#### GUI
1. Electron
2. SignalR
3. WPF 

#### restful 规范
``` http
请求成功
GET 200 OK
POST 201 Created
PUT 202 Accepted
DELETE 204 No Content

200 OK - [GET]：服务器成功返回用户请求的数据，该操作是幂等的（Idempotent）。
201 CREATED - [POST/PUT/PATCH]：用户新建或修改数据成功。
202 Accepted - [*]：表示一个请求已经进入后台排队（异步任务）
204 NO CONTENT - [DELETE]：用户删除数据成功。
400 INVALID REQUEST - [POST/PUT/PATCH]：用户发出的请求有错误，服务器没有进行新建或修改数据的操作，该操作是幂等的。
401 Unauthorized - [*]：表示用户没有权限（令牌、用户名、密码错误）。
403 Forbidden - [*] 表示用户得到授权（与401错误相对），但是访问是被禁止的。
404 NOT FOUND - [*]：用户发出的请求针对的是不存在的记录，服务器没有进行操作，该操作是幂等的。
406 Not Acceptable - [GET]：用户请求的格式不可得（比如用户请求JSON格式，但是只有XML格式）。
410 Gone -[GET]：用户请求的资源被永久删除，且不会再得到的。
422 Unprocesable entity - [POST/PUT/PATCH] 当创建一个对象时，发生一个验证错误。
500 INTERNAL SERVER ERROR - [*]：服务器发生错误，用户将无法判断发出的请求是否成功。
```
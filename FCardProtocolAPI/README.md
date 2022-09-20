# 人脸机、指纹机、控制板

## **webAPI**

## **控制接口**

**代码下载地址：**

https://www.facedata.vip:9901/DotNetApiService.zip

## 协议格式定义

#### 通讯方式

本协议基于https 通讯方式

#### 协议格式

协议格式基于JSON字符串，命令所需的参数都放到body中提交

#### 请求地址格式说明

测试地址：

```
https://www.facedata.vip:9901
```

地址格式：

```
https://ip:port/{sn}/{command}
```

sn为设备SN编号(16位)，command需要执行的命令：

```

 https://www.facedata.vip:9901/FC-8300T21076163/opendoor

```

上面的URL地址表示发送开门命令

#### 使用前准备

##### 人脸设备设置

广域网：设置设备服务器IP为8.142.71.221端口为9902

<img src="https://www.facedata.vip:9901/images/server.png" width="50%">

局域网：调用“注册设备”接口进行设备注册，注册完成之后才能通过SN调用命令

##### 指纹机、控制板、人脸机局域网设置软件及代码下载地址

https://gitee.com/GZFCARD/iotwebsoketserver/blob/master/AutoSetFCARDIP.zip



#### 命令响应

| 字段    | 类型   | 必填 | 描述                                 |
| ------- | ------ | ---- | ------------------------------------ |
| command | string | 是   | 返回执行的命令方法                   |
| status  | int    | 是   | 命令执行状态                         |
| message | string | 是   | 命令消息                             |
| guid    | string | 否   | 暂不使用                             |
| data    | object | 否   | 根据不同命令，响应的结果也与之对应。 |

status：

| 返回值 | 描述         |
| ------ | ------------ |
| 200    | 命令成功     |
| 100    | 命令超时     |
| 101    | 通讯密码错误 |
| 102    | 命令错误     |
| 103    | 命令参数错误 |
| 104    | 连接设备超时 |



##通用命令

### 注册设备

命令：[Register](/ApiTest/register.html)

Body参数:

| 字段 | 类型 | 必填 | 描述 |
| --- | --- | --- | --- |
| sn | String | 是 | 设备sn（获取设备列表时可空） |
| ip | String | 否 | 设备ip |
| port | Int | 否 | 设备端口 |
示例：

```
 { 
     "sn": FC-8600T20124011,
     "ip": "192.168.1.130",
     "port": 8101 
 }
```





### **获取已注册列表**

命令：[getDevices](/ApiTest/getDevices.html)

Body参数：无

![输入图片说明](/images/getDevices.png"getDevices.png")

响应参数：

```
[
    {
        "sn":"FC-8300T21076163",
        "ip":"192.168.1.62",
        "port":"51283"
    }
]
```




### 远程开门

命令：[opendoor](/ApiTest/openDoor.html)

Body参数：无

![输入图片说明](/images/openDoor.png"openDoor.png")


### **远程关门**

命令：closedoor

示例：[https://www.facedata.vip:9901/ApiTest/closeDoor.html](https://www.facedata.vip:9901/ApiTest/closeDoor.html)

Body参数：无

![输入图片说明](https://www.facedata.vip:9901/images/closeDoor.png "closeDoor.png")

### **门常开**

命令：holddoor

示例：[https://www.facedata.vip:9901/ApiTest/holdDoor.html](https://www.facedata.vip:9901/ApiTest/holdDoor.html)

Body参数：无

![输入图片说明](https://www.facedata.vip:9901/images/holdDoor.png "holdDoor.png")




### 获取记录

命令：[getRecord](/ApiTest/getRecord.html)

Body参数：无

![输入图片说明](https://www.facedata.vip:9901/images/getRecord.png "getRecord.png")

返回字段：

| 字段 | 类型 | 必填 | 描述 |
| --- | --- | --- | --- |
| Quantity | Int | 是 | 读取数量 |
| readable | int | 是 | 剩余新记录数量 |
| CardTransactionReadIndex | string | 是 | 当前记录位置 |
| BodyTemperatureReadIndex | string | 是 | 当前体温记录位置 |
| TransactionList | Array | 否 | 记录列表 |

TransactionList:

| 字段 | 类型 | 必填 | 描述 |
| --- | --- | --- | --- |
| RecordNumber | Int | 是 | 记录唯一编号 |
| UserCode | long | 否 | 用户编号 |
| RecordImage | string | 否 | 记录图片Base64 |
| Accesstype | Byte | 否 | 出入类型：1--表示进门；2--表示出门 |
| BodyTemperature | double | 否 | 体温 |
| RecordDate | datetime | 是 | 记录时间 |
| RecordType | Int | 是 | 记录类型1 认证记录2 门磁记录3 系统日志4 体温记录 |
| RecordMsg | string | 是 | 记录消息 |


### 重置记录

当记录需要从头获取时候，可以调用该接口

命令：[resetRecord](/ApiTest/resetRecord.html)

Body参数：无

![输入图片说明](https://www.facedata.vip:9901/images/resetRecord.png "resetRecord.png")



### 消息推送

消息推送协议基于websocket 通讯方式

连接成功之后会返回连接成功的消息

返回的消息格式为JSON

连接地址格式:ws://8.142.71.221:9903/WebSocket

记录格式：参考7、获取记录

测试客户端：[SocketClient.html](/SocketClient.html)



##人脸指纹设备

### 添加人员

命令：[addPerson](/ApiTest/addPerson.html)

Body参数：

| 字段        | 类型   | 必填 | 描述                                               |
| ----------- | ------ | ---- | -------------------------------------------------- |
| name        | string | 是   | 人员姓名                                           |
| userCode    | int    | 是   | 用户编号                                           |
| code        | string | 否   | 人员编号                                           |
| cardData    | string | 否   | 卡号，取值范围0x1-0xFFFFFFFF                       |
| password    | string | 否   | 卡密码,无密码不填。密码是4-8位的数字               |
| job         | string | 否   | 人员职务                                           |
| dept        | string | 否   | 人员部门                                           |
| identity    | int    | 否   | 用户身份0 -- 普通用户1 -- 管理员                   |
| cardStatus  | int    | 否   | 卡片状态0：正常状态；1：挂失；2：黑名单；3：已删除 |
| cardType    | int    | 否   | 卡片类型0 -- 普通卡1 -- 常开                       |
| enterStatus | int    | 否   | 出入标记0 出入有效1 入有效2 出有效                 |
| expiry      | string | 否   | 出入截止日期，最大2089年12月31日                   |
| openTimes   | int    | 否   | 有效次数,取值范围：0-65535;65535表示无限制         |
| faceImage   | String | 否   | 人脸图片或者指纹特征码                             |
| fp          | array  | 否   | 指纹特征码Base64，可以有多个                       |

发送示例：

```
{
	"name": "user name",
	"userCode": "10000",
	"code": "10000",
	"cardData": 5,
	"password": "12345678",
	"job": 1,
	"dept": 1,
	"identity": 0,
	"cardStatus": 0,
	"cardType": 0,
	"enterStatus": 0,
	"expiry": "2021-10-10 00:00:00",
	"openTimes": 65535,
	"faceImage": "/9j/4AAQSkZJRgABAQAAAQABAAD.....",
	"fp": [
		"/9j/4AAQSkZJRgABAQAAAQABAAD......",
		"/9j/4AAQSkZJRgABAQAAAQABAAD......"
	]
}
```

响应参数：

```
{
    "UserUploadStatus":false,
    "IdDataRepeatUser":1001,
    "IdDataUploadStatus":4
}
```

| 字段               | 类型 | 必填 | 描述                                                         |
| ------------------ | ---- | ---- | ------------------------------------------------------------ |
| UserUploadStatus   | bool | 是   | 上传成功状态：true---上传成功；false--上传失败               |
| IdDataRepeatUser   | uint | 是   | 人员重复用户号（IdDataUploadStatus=4时返回重复与当前人员重复的用户号） |
| IdDataUploadStatus | int  | 是   | 上传状态：1--上传完毕；2--特征码无法识别；3--人员照片不可识别；4--人员照片或特征码重复；0--没有 |

### 删除人员

命令：[deletePerson](/ApiTest/deletePerson.html)

body参数：

| 字段          | 类型  | 必填 | 描述         |
| ------------- | ----- | ---- | ------------ |
| userCodeArray | array | 是   | 用户编号数组 |

发送示例：

```
{
    "userCodeArray":["10000","10001"]
}
```



### 获取人员

命令：[getPersonDetail](/ApiTest/getPersonDetail.html)

body参数：

| 字段     | 类型   | 必填 | 描述     |
| -------- | ------ | ---- | -------- |
| userCode | string | 是   | 用户编号 |

发送示例：

```
{
    "userCodeArray":["10000","10001"]
}
```

响应参数：

```
{
	"name": "user name",
	"userCode": "10000",
	"code": "10000",
	"cardData": 5,
	"password": "12345678",
	"job": 1,
	"dept": 1,
	"identity": 0,
	"cardStatus": 0,
	"cardType": 0,
	"enterStatus": 0,
	"expiry": "2021-10-10 00:00:00",
	"openTimes": 65535,
	"faceImage": "/9j/4AAQSkZJRgABAQAAAQABAAD.....",
	"fp": [
		"/9j/4AAQSkZJRgABAQAAAQABAAD......",
		"/9j/4AAQSkZJRgABAQAAAQABAAD......"
	]
}
```



### 设置工作参数

命令：[setWorkParam](/ApiTest/setWorkParam.html)

Body参数：

| 字段         | 类型     | 必填 | 描述                                                         |
| ------------ | -------- | ---- | ------------------------------------------------------------ |
| name         | string   | 是   | 设备名称，最大30个字符（需要修改设备名称时，进出类别也需要填写，修改进出类别亦是如此） |
| door         | byte     | 是   | 进出类别：0--进门；1--出门                                   |
| maker        | objcet   | 否   | 设备制造商信息                                               |
| language     | byte     | 否   | 语言1 -中文，2 -英文，3 -繁体，4 -法语，5 -俄语，6 -葡萄牙语，7 -西班牙语，8 -意大利语，9 -日语，10 -韩语，11 -泰语，12 -阿拉伯语， |
| volume       | byte     | 否   | 音量音量取值范围：0-10；0--关闭声音；10--最大声音默认值：10  |
| menuPassword | string   |      | 菜单密码仅支持4-8位数字密码                                  |
| savePhoto    | byte     |      | 现场照片保存开关取值范围：0--禁止保存；1--保存现场照片       |
| msgPush      | byte     |      | 消息推送开关0--禁用；1--启用启用后，有验证开门、系统报警等事件发生时就会在链路上推送消息，连接断开时缓存离线消息，连接建立后继续推送 |
| time         | datetime |      | 日期时间同步yyyy-MM-dd HH:mm:ss                              |

Maker：

| 字段         | 类型   | 必填 | 描述           |
| ------------ | ------ | ---- | -------------- |
| manufacturer | string | 是   | 设备制造商名称 |
| webAddr      | string | 是   | 设备制造商网站 |
| deliveryDate | string | 是   | 设备出厂时间   |

发送示例：

```
{
	"name": "device name",
	"door": 0,
	"maker": {
		"manufacturer": "manufacturer name",
		"webAddr": "httpss://www.githubs.cn/",
		"deliveryDate": "2021-10-10 00:00:00"
	},
	"language": 2,
	"volume": 5,
	"menuPassword": "12345678",
	"savePhoto": 1,
	"msgPush": 1,
	"time": "2021-10-10 00:00:00"
}
```



### 获取工作参数

命令：[getWorkParam](/ApiTest/getWorkParam.html)

Body参数：无

![输入图片说明](https://www.facedata.vip:9901/images/getWorkParam.png "getWorkParam.png")

## 控制板设备

### 添加卡号

命令：[addCard](/ApiTest/addCard.html)

body参数：

| 字段     | 类型      | 必填 | 描述                                      |
| -------- | --------- | ---- | ----------------------------------------- |
| cards    | ArrayList | 是   | 卡信息列表                                |
| areaType | int       | 否   | 存储区域：0--非排序区；1--排序区；默认--0 |

cards:

| 字段      | 类型   | 必填 | 描述                                                         |
| --------- | ------ | ---- | ------------------------------------------------------------ |
| cardData  | string | 是   | 卡号                                                         |
| password  | string | 否   | 卡密码：如果添加了卡密码，刷卡是需要键盘输入密码才能开门     |
| expiry    | string | 否   | 有效期：默认有效期是2089-12-31 23:59:59                      |
| doors     | object | 否   | 开门权限：true--有开门权限；false--无开门权限;默认拥有所有权限 |
| openTimes | int    | 否   | 开门次数：0--没有开门次数，65535---无限制;默认--无限制       |

doors：

| 字段  | 类型 | 必填 | 描述  |
| ----- | ---- | ---- | ----- |
| door1 | bool | 是   | 门号1 |
| door2 | bool | 是   | 门号2 |
| door3 | bool | 是   | 门号3 |
| door4 | bool | 是   | 门号4 |

发送示例：

```
{
	"cards": [{
		"cardData": "1463821531",
		"password": "12345678",
		"expiry": "2089-12-31 23:59:59",
		"doors": {
			"door1": true,
			"door2": true,
			"door3": true,
			"door4": true
		},
		"openTimes": 65535

	}],
	"areaType": 0
}
```



### 删除卡号

命令：[deleteCard](/ApiTest/deleteCard.html)

body参数：

| 字段      | 类型  | 必填 | 描述               |
| --------- | ----- | ---- | ------------------ |
| CardArray | Array | 是   | 需要删除的卡号数组 |

发送示例：

```
{
	"CardArray": ["1463821531", "1483923632"]
}
```


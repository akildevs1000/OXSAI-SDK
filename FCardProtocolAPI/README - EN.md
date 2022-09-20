# Face machine, fingerprint machine, control panel

## **webAPI**

## Control Interface

Code download address:

https://www.facedata.vip:9901/DotNetApiService.zip

## Protocol Format Definition

#### Communication method

This agreement is based on https communication

#### Protocol format

The protocol format is based on JSON strings, and the parameters required by the command are placed in the body for submission

#### Request address format description

Test address:

```
https://www.facedata.vip:9901
```

Address format:

```
https://ip:port/{sn}/{command}
```

sn is the device SN number (16 bits), the command that command needs to execute:

```

 https://www.facedata.vip:9901/FC-8300T21076163/opendoor

```

The above URL address means sending the door opening command

#### Preparation before use

##### Face Device Settings

WAN: Set the device server IP to 8.142.71.221 and the port to 9902

<img src="https://www.facedata.vip:9901/images/server.png" width="50%">

Local area network: Call the "Register Device" interface to register the device, after the registration is completed, the command can be called through the SN

##### Fingerprint machine, control panel, face machine LAN setting software and code download address

https://gitee.com/GZFCARD/iotwebsoketserver/blob/master/AutoSetFCARDIP.zip



#### Command response

| Field   | type   | Required | Describe                                                     |
| ------- | ------ | -------- | ------------------------------------------------------------ |
| command | string | Yes      | returns the executed command method                          |
| status  | int    | Yes      | command execution status                                     |
| message | string | Yes      | command message                                              |
| guid    | string | No       | do not use                                                   |
| data    | object | No       | Depending on the command, the result of the response also corresponds to it. |

status：

| Return value | Describe                       |
| ------------ | ------------------------------ |
| 200          | command succeeded              |
| 100          | command timed out              |
| 101          | Communication password error   |
| 102          | command error                  |
| 103          | Command parameter error        |
| 104          | Connection to device timed out |



##General command

### Register the device

Order：[Register](/ApiTest/register.html)

Body parameter:

| Field | Type | Required | Describe |
| --- | --- | --- | --- |
| sn | String | Yes | Device sn (empty when getting device list) |
| ip | String | No       | Device ip |
| port | Int | No | Device port |
|Example:||||

```
 { 
     "sn": FC-8600T20124011,
     "ip": "192.168.1.130",
     "port": 8101 
 }
```





### Get registered list

Order：[getDevices](/ApiTest/getDevices.html)

Body parameter：none

![Enter image description](/images/getDevices.png"getDevices.png")

Response parameters:

```
[
    {
        "sn":"FC-8300T21076163",
        "ip":"192.168.1.62",
        "port":"51283"
    }
]
```




### Remote door

Order：[opendoor](/ApiTest/openDoor.html)

Body parameter：none

![Enter image description](/images/openDoor.png"openDoor.png")


### **Remote closing**

Order：closedoor

Example：[https://www.facedata.vip:9901/ApiTest/closeDoor.html](https://www.facedata.vip:9901/ApiTest/closeDoor.html)

Body parameter：none

![Enter image description](https://www.facedata.vip:9901/images/closeDoor.png "closeDoor.png")

### **Door always open**

Order：holddoor

Example：[https://www.facedata.vip:9901/ApiTest/holdDoor.html](https://www.facedata.vip:9901/ApiTest/holdDoor.html)

Body parameter：none

![Enter image description](https://www.facedata.vip:9901/images/holdDoor.png "holdDoor.png")




### Get records

Order：[getRecord](/ApiTest/getRecord.html)

Body parameter：none

![Enter image description](https://www.facedata.vip:9901/images/getRecord.png "getRecord.png")

Return fields:

| Fields | Type   | Required | Describe |
| --- | --- | --- | --- |
| Quantity | Int | Yes | Number of reads |
| readable | int | Yes | Number of new records remaining |
| CardTransactionReadIndex | string | Yes | Current recording position |
| BodyTemperatureReadIndex | string | Yes | Current temperature record location |
| TransactionList | Array | No       | Tecord list |

TransactionList:

| Field | Type | Required | Describe |
| --- | --- | --- | --- |
| RecordNumber | Int | Yes | Record unique number |
| UserCode | long | No | User ID |
| RecordImage | string | No | Record image Base64 |
| Accesstype | Byte | No | Access type: 1-- means entering the door; 2-- means going out |
| BodyTemperature | double | No | Body temperature |
| RecordDate | datetime | Yes | Record time                                                  |
| RecordType | Int | Yes | Record Type 1 Authentication Record 2 Door Magnetic Record 3 System Log 4 Body Temperature Record |
| RecordMsg | string | Yes | Log message                                                  |


### Reset record

This interface can be called when the record needs to be obtained from scratch

Order：[resetRecord](/ApiTest/resetRecord.html)

Body parameter：none

![Enter image description](https://www.facedata.vip:9901/images/resetRecord.png "resetRecord.png")

Message push

The message push protocol is based on the websocket communication method

After the connection is successful, a connection successful message will be returned
The returned message format is JSON

Connection address format: ws://8.142.71.221:9903/WebSocket

Record format: refer to 7. Obtaining records

Test client：[SocketClient.html](/SocketClient.html)



##Face fingerprint device

### Add people

Order：[addPerson](/ApiTest/addPerson.html)

Body parameter：

| Field       | Type   | Required | Describe                                                     |
| ----------- | ------ | -------- | ------------------------------------------------------------ |
| name        | string | Yes      | Person's name                                                |
| userCode    | int    | Yes      | User ID                                                      |
| code        | string | No       | Personnel number                                             |
| cardData    | string | No       | Card number, value range0x1-0xFFFFFFFF                       |
| password    | string | No       | Card password, no password is left blank. Password is 4-8 digits |
| job         | string | No       | Personnel position                                           |
| dept        | string | No       | Personnel department                                         |
| identity    | int    | No       | User Identity 0 -- Normal User 1 -- Administrator            |
| cardStatus  | int    | No       | Card status 0: normal; 1: report loss; 2: blacklist; 3: deleted |
| cardType    | int    | No       | Card Type 0 -- Normal Card 1 -- Normally Open                |
| enterStatus | int    | No       | In-out flag 0 In-out valid 1 In-out valid 2 Out-valid        |
| expiry      | string | No       | Deadline for entry and exit, maximum December 31, 2089       |
| openTimes   | int    | No       | Valid times, value range: 0-65535; 65535 means unlimited     |
| faceImage   | String | No       | Face picture or fingerprint feature code                     |
| fp          | array  | No       | Fingerprint signature Base64, there can be multiple          |

Send example：

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

Response parameter：

```
{
    "UserUploadStatus":false,
    "IdDataRepeatUser":1001,
    "IdDataUploadStatus":4
}
```

| Field              | Type | Required | Describe                                                     |
| ------------------ | ---- | -------- | ------------------------------------------------------------ |
| UserUploadStatus   | bool | Yes      | Upload success status: true---upload successful; false-upload failed |
| IdDataRepeatUser   | uint | Yes      | Person with duplicate user ID (when IdDataUploadStatus=4, it will return the duplicate user ID of the current person) |
| IdDataUploadStatus | int  | Yes      | Upload status: 1--upload completed; 2--feature code unrecognizable; 3--person photo unrecognizable; 4--person photo or feature code duplicate; 0--no |

### Delete people

Order：[deletePerson](/ApiTest/deletePerson.html)

body parameter：

| Field         | Type  | Required | Describe      |
| ------------- | ----- | -------- | ------------- |
| userCodeArray | array | Yes      | User ID array |

Send example：

```
{
    "userCodeArray":["10000","10001"]
}
```



### Get people

Order：[getPersonDetail](/ApiTest/getPersonDetail.html)

body parameter：

| Field    | Type   | Required | Describe |
| -------- | ------ | -------- | -------- |
| userCode | string | Yes      | user ID  |

Send example：

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



### Set working parameters

Order：[setWorkParam](/ApiTest/setWorkParam.html)

Body parameter：

| Field        | Type     | Required | Describe                                                     |
| ------------ | -------- | -------- | ------------------------------------------------------------ |
| name         | string   | Yes      | Device name, up to 30 characters (when the device name needs to be modified, the entry and exit category also needs to be filled in, and the same is true for modifying the entry and exit category) |
| door         | byte     | Yes      | In and out category: 0--into the door; 1--out of the door    |
| maker        | objcet   | No       | Device Manufacturer Information                              |
| language     | byte     | No       | Languages 1 - Chinese, 2 - English, 3 - Traditional, 4 - French, 5 - Russian, 6 - Portuguese, 7 - Spanish, 8 - Italian, 9 - Japanese, 10 - Korean, 11 - Thai, 12 - Arabic |
| volume       | byte     | No       | Volume volume value range: 0-10; 0--turn off the sound; 10--maximum sound Default value: 10 |
| menuPassword | string   |          | Menu password only supports 4-8 digit password               |
| savePhoto    | byte     |          | The value range of the on-site photo save switch: 0--no saving; 1--save on-site photos |
| msgPush      | byte     |          | Message push switch 0--disable; 1--enable, when there are events such as verification door opening, system alarm, etc., the message will be pushed on the link, and the offline message will be cached when the connection is disconnected, and continue to push after the connection is established |
| time         | datetime |          | datetime sync yyyy-MM-dd HH:mm:ss                            |

Maker：

| Field        | Type   | Required | Describe                    |
| ------------ | ------ | -------- | --------------------------- |
| manufacturer | string | Yes      | Device manufacturer name    |
| webAddr      | string | Yes      | Device manufacturer website |
| deliveryDate | string | Yes      | Equipment factory time      |

Send example：

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



### Get working parameters

Order：[getWorkParam](/ApiTest/getWorkParam.html)

Body parameter：None

![Enter image description](https://www.facedata.vip:9901/images/getWorkParam.png "getWorkParam.png")

## Control panel device

### Add card number

Order：[addCard](/ApiTest/addCard.html)

body parameter：

| Field    | Type      | Required | Describe                                                   |
| -------- | --------- | -------- | ---------------------------------------------------------- |
| cards    | ArrayList | Yes      | Card Information List                                      |
| areaType | int       | No       | Storage area: 0--unsorted area; 1--sorted area; default--0 |

cards:

| Field     | Type   | Required | Describe                                                     |
| --------- | ------ | -------- | ------------------------------------------------------------ |
| cardData  | string | Yes      | Card number                                                  |
| password  | string | No       | Card password: If a card password is added, swiping the card requires the keyboard to enter the password to open the door |
| expiry    | string | No       | Validity period: The default validity period is 2089-12-31 23:59:59 |
| doors     | object | No       | Permission to open the door: true--with permission to open the door; false--without permission to open the door; all permissions are by default |
| openTimes | int    | No       | Number of door openings: 0--no opening times, 65535--unlimited; default--unlimited |

doors：

| Field | Type | Required | Describe      |
| ----- | ---- | -------- | ------------- |
| door1 | bool | Yes      | Door number 1 |
| door2 | bool | Yes      | Door number 2 |
| door3 | bool | Yes      | Door number 3 |
| door4 | bool | Yes      | Door number 4 |

Send example：

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



### Delete card number

Order：[deleteCard](/ApiTest/deleteCard.html)

body parameter：

| Field     | Type  | Required | Describe                        |
| --------- | ----- | -------- | ------------------------------- |
| CardArray | Array | Yes      | Array of card numbers to delete |

Send example：

```
{
	"CardArray": ["1463821531", "1483923632"]
}
```


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCardProtocolAPI.Common
{
    public class MessageType
    {
        public static List<string[]> TransactionCodeNameList = new();

        public static List<string[]> FaceTransactionCodeList = new();

        public static List<string[]> CardTransactionCodeList = new List<string[]>();
        static MessageType()
        {
            if (TransactionCodeNameList.Count == 0)
            {
                var mCardTransactionList = new string[256];
                var mDoorSensorTransactionList = new string[256];
                var mSystemTransactionList = new string[256];
                TransactionCodeNameList.Add(null);//0是没有的
                TransactionCodeNameList.Add(mCardTransactionList);
                TransactionCodeNameList.Add(mDoorSensorTransactionList);
                TransactionCodeNameList.Add(mSystemTransactionList);
                mCardTransactionList[1] = "刷卡验证";//
                mCardTransactionList[2] = "指纹验证";//------------卡号为密码
                mCardTransactionList[3] = "人脸验证";//
                mCardTransactionList[4] = "指纹 + 刷卡";//
                mCardTransactionList[5] = "人脸 + 指纹";//
                mCardTransactionList[6] = "人脸 + 刷卡";//   ---  常开工作方式中，刷卡进入常开状态
                mCardTransactionList[7] = "刷卡 + 密码";//  --  多卡验证组合完毕后触发
                mCardTransactionList[8] = "人脸 + 密码";//
                mCardTransactionList[9] = "指纹 + 密码";//
                mCardTransactionList[10] = "手动输入用户号加密码验证";//
                mCardTransactionList[11] = "指纹+刷卡+密码";//
                mCardTransactionList[12] = "人脸+刷卡+密码";//
                mCardTransactionList[13] = "人脸+指纹+密码";//  --  不开门
                mCardTransactionList[14] = "人脸+指纹+刷卡";//
                mCardTransactionList[15] = "重复验证";//
                mCardTransactionList[16] = "有效期过期";//
                mCardTransactionList[17] = "开门时段过期";//------------卡号为错误密码
                mCardTransactionList[18] = "节假日时不能开门";//----卡号为卡号。
                mCardTransactionList[19] = "未注册用户";//
                mCardTransactionList[20] = "探测锁定";//
                mCardTransactionList[21] = "有效次数已用尽";//
                mCardTransactionList[22] = "锁定时验证，禁止开门";//
                mCardTransactionList[23] = "挂失卡";//
                mCardTransactionList[24] = "黑名单卡";//
                mCardTransactionList[25] = "免验证开门 -- 按指纹时用户号为0，刷卡时用户号是卡号";//
                mCardTransactionList[26] = "禁止刷卡验证  --  【权限认证方式】中禁用刷卡时";//
                mCardTransactionList[27] = "禁止指纹验证  --  【权限认证方式】中禁用指纹时";//
                mCardTransactionList[28] = "控制器已过期";//
                mCardTransactionList[29] = "验证通过—有效期即将过期";//
                mCardTransactionList[30] = "体温异常，拒绝进入";//

                mDoorSensorTransactionList[1] = "开门";//
                mDoorSensorTransactionList[2] = "关门";//
                mDoorSensorTransactionList[3] = "进入门磁报警状态";//
                mDoorSensorTransactionList[4] = "退出门磁报警状态";//
                mDoorSensorTransactionList[5] = "门未关好";//
                mDoorSensorTransactionList[6] = "使用按钮开门";//
                mDoorSensorTransactionList[7] = "按钮开门时门已锁定";//
                mDoorSensorTransactionList[8] = "按钮开门时控制器已过期";//

                mSystemTransactionList[1] = "软件开门";//
                mSystemTransactionList[2] = "软件关门";//
                mSystemTransactionList[3] = "软件常开";//
                mSystemTransactionList[4] = "控制器自动进入常开";//
                mSystemTransactionList[5] = "控制器自动关闭门";//
                mSystemTransactionList[6] = "长按出门按钮常开";//
                mSystemTransactionList[7] = "长按出门按钮常闭";//
                mSystemTransactionList[8] = "软件锁定";//
                mSystemTransactionList[9] = "软件解除锁定";//
                mSystemTransactionList[10] = "控制器定时锁定--到时间自动锁定";//
                mSystemTransactionList[11] = "控制器定时锁定--到时间自动解除锁定";//
                mSystemTransactionList[12] = "报警--锁定";//
                mSystemTransactionList[13] = "报警--解除锁定";//
                mSystemTransactionList[14] = "非法认证报警";//
                mSystemTransactionList[15] = "门磁报警";//
                mSystemTransactionList[16] = "胁迫报警";//
                mSystemTransactionList[17] = "开门超时报警";//
                mSystemTransactionList[18] = "黑名单报警";//
                mSystemTransactionList[19] = "消防报警";//
                mSystemTransactionList[20] = "防拆报警";//
                mSystemTransactionList[21] = "非法认证报警解除";//
                mSystemTransactionList[22] = "门磁报警解除";//
                mSystemTransactionList[23] = "胁迫报警解除";//
                mSystemTransactionList[24] = "开门超时报警解除";//
                mSystemTransactionList[25] = "黑名单报警解除";//
                mSystemTransactionList[26] = "消防报警解除";//
                mSystemTransactionList[27] = "防拆报警解除";//
                mSystemTransactionList[28] = "系统加电";//
                mSystemTransactionList[29] = "系统错误复位（看门狗）";//
                mSystemTransactionList[30] = "设备格式化记录";//
                mSystemTransactionList[31] = "读卡器接反";//
                mSystemTransactionList[32] = "读卡器线路未接好";//
                mSystemTransactionList[33] = "无法识别的读卡器";//
                mSystemTransactionList[34] = "网线已断开";//
                mSystemTransactionList[35] = "网线已插入";//
                mSystemTransactionList[36] = "WIFI 已连接";//
                mSystemTransactionList[37] = "WIFI 已断开";//
            }

            if (FaceTransactionCodeList.Count == 0)
            {
                var CardTransactionList = new string[50];
                CardTransactionCodeList.Add(null);//0是没有的
                CardTransactionCodeList.Add(CardTransactionList);
                CardTransactionList[1] = "合法开门";
                CardTransactionList[2] = "密码开门------------卡号为密码";
                CardTransactionList[3] = "卡加密码";
                CardTransactionList[4] = "手动输入卡加密码";
                CardTransactionList[5] = "首卡开门";
                CardTransactionList[6] = "门常开---常开工作方式中，刷卡进入常开状态";
                CardTransactionList[7] = "多卡开门--多卡验证组合完毕后触发";
                CardTransactionList[8] = "重复读卡";
                CardTransactionList[9] = "有效期过期";
                CardTransactionList[10] = "开门时段过期";
                CardTransactionList[11] = "节假日无效";
                CardTransactionList[12] = "未注册卡";
                CardTransactionList[13] = "巡更卡--不开门";
                CardTransactionList[14] = "探测锁定";
                CardTransactionList[15] = "无有效次数";
                CardTransactionList[16] = "防潜回";
                CardTransactionList[17] = "密码错误------------卡号为错误密码";
                CardTransactionList[18] = "密码加卡模式密码错误----卡号为卡号。";
                CardTransactionList[19] = "锁定时(读卡)或(读卡加密码)开门";
                CardTransactionList[20] = "锁定时(密码开门)";
                CardTransactionList[21] = "首卡未开门";
                CardTransactionList[22] = "挂失卡";
                CardTransactionList[23] = "黑名单卡";
                CardTransactionList[24] = "门内上限已满，禁止入门。";
                CardTransactionList[25] = "开启防盗布防状态(设置卡)";
                CardTransactionList[26] = "撤销防盗布防状态(设置卡)";
                CardTransactionList[27] = "开启防盗布防状态(密码)";
                CardTransactionList[28] = "撤销防盗布防状态(密码)";
                CardTransactionList[29] = "互锁时(读卡)或(读卡加密码)开门";
                CardTransactionList[30] = "互锁时(密码开门)";
                CardTransactionList[31] = "全卡开门";
                CardTransactionList[32] = "多卡开门--等待下张卡";
                CardTransactionList[33] = "多卡开门--组合错误";
                CardTransactionList[34] = "非首卡时段刷卡无效";
                CardTransactionList[35] = "非首卡时段密码无效";
                CardTransactionList[36] = "禁止刷卡开门--【开门认证方式】验证模式中禁用了刷卡开门时";
                CardTransactionList[37] = "禁止密码开门--【开门认证方式】验证模式中禁用了密码开门时";
                CardTransactionList[38] = "门内已刷卡，等待门外刷卡。（门内外刷卡验证）";
                CardTransactionList[39] = "门外已刷卡，等待门内刷卡。（门内外刷卡验证）";
                CardTransactionList[40] = "请刷管理卡(在开启管理卡功能后提示)(电梯板)";
                CardTransactionList[41] = "请刷普通卡(在开启管理卡功能后提示)(电梯板)";
                CardTransactionList[42] = "首卡未读卡时禁止密码开门。";
                CardTransactionList[43] = "控制器已过期_刷卡";
                CardTransactionList[44] = "控制器已过期_密码";
                CardTransactionList[45] = "合法卡开门—有效期即将过期";
                CardTransactionList[46] = "拒绝开门--区域反潜回失去主机连接。";
                CardTransactionList[47] = "拒绝开门--区域互锁，失去主机连接";
                CardTransactionList[48] = "区域防潜回--拒绝开门";
                CardTransactionList[49] = "区域互锁--有门未关好，拒绝开门";
            }

            if (CardTransactionCodeList.Count == 0)
            {
             var   mCardTransactionList = new string[256];
                var mButtonTransactionList = new string[256];
                var mDoorSensorTransactionList = new string[256];
                var mSoftwareTransactionList = new string[256];
                var mAlarmTransactionList = new string[256];
                var mSystemTransactionList = new string[256];

                var mTransactionCodeNameList = new List<string[]>(10);
                mTransactionCodeNameList.Add(null);//0是没有的
                mTransactionCodeNameList.Add(mCardTransactionList);
                mTransactionCodeNameList.Add(mButtonTransactionList);
                mTransactionCodeNameList.Add(mDoorSensorTransactionList);
                mTransactionCodeNameList.Add(mSoftwareTransactionList);
                mTransactionCodeNameList.Add(mAlarmTransactionList);
                mTransactionCodeNameList.Add(mSystemTransactionList);

                mCardTransactionList[1] = "合法开门";//
                mCardTransactionList[2] = "密码开门";//------------卡号为密码
                mCardTransactionList[3] = "卡加密码";//
                mCardTransactionList[4] = "手动输入卡加密码";//
                mCardTransactionList[5] = "首卡开门";//
                mCardTransactionList[6] = "门常开";//   ---  常开工作方式中，刷卡进入常开状态
                mCardTransactionList[7] = "多卡开门";//  --  多卡验证组合完毕后触发
                mCardTransactionList[8] = "重复读卡";//
                mCardTransactionList[9] = "有效期过期";//
                mCardTransactionList[10] = "开门时段过期";//
                mCardTransactionList[11] = "节假日无效";//
                mCardTransactionList[12] = "未注册卡";//
                mCardTransactionList[13] = "巡更卡";//  --  不开门
                mCardTransactionList[14] = "探测锁定";//
                mCardTransactionList[15] = "无有效次数";//
                mCardTransactionList[16] = "防潜回";//
                mCardTransactionList[17] = "密码错误";//------------卡号为错误密码
                mCardTransactionList[18] = "密码加卡模式密码错误";//----卡号为卡号。
                mCardTransactionList[19] = "锁定时(读卡)或(读卡加密码)开门";//
                mCardTransactionList[20] = "锁定时(密码开门)";//
                mCardTransactionList[21] = "首卡未开门";//
                mCardTransactionList[22] = "挂失卡";//
                mCardTransactionList[23] = "黑名单卡";//
                mCardTransactionList[24] = "门内上限已满，禁止入门。";//
                mCardTransactionList[25] = "开启防盗布防状态(设置卡)";//
                mCardTransactionList[26] = "撤销防盗布防状态(设置卡)";//
                mCardTransactionList[27] = "开启防盗布防状态(密码)";//
                mCardTransactionList[28] = "撤销防盗布防状态(密码)";//
                mCardTransactionList[29] = "互锁时(读卡)或(读卡加密码)开门";//
                mCardTransactionList[30] = "互锁时(密码开门)";//
                mCardTransactionList[31] = "全卡开门";//
                mCardTransactionList[32] = "多卡开门--等待下张卡";//
                mCardTransactionList[33] = "多卡开门--组合错误";//
                mCardTransactionList[34] = "非首卡时段刷卡无效";//
                mCardTransactionList[35] = "非首卡时段密码无效";//
                mCardTransactionList[36] = "禁止刷卡开门";//  --  【开门认证方式】验证模式中禁用了刷卡开门时
                mCardTransactionList[37] = "禁止密码开门";//  --  【开门认证方式】验证模式中禁用了密码开门时
                mCardTransactionList[38] = "门内已刷卡，等待门外刷卡。";//（门内外刷卡验证）
                mCardTransactionList[39] = "门外已刷卡，等待门内刷卡。";//（门内外刷卡验证）
                mCardTransactionList[40] = "请刷管理卡";//(在开启管理卡功能后提示)(电梯板)
                mCardTransactionList[41] = "请刷普通卡";//(在开启管理卡功能后提示)(电梯板)
                mCardTransactionList[42] = "首卡未读卡时禁止密码开门。";//
                mCardTransactionList[43] = "控制器已过期_刷卡";//
                mCardTransactionList[44] = "控制器已过期_密码";//
                mCardTransactionList[45] = "合法卡开门—有效期即将过期";//
                mCardTransactionList[46] = "拒绝开门--区域反潜回失去主机连接。";//
                mCardTransactionList[47] = "拒绝开门--区域互锁，失去主机连接";//
                mCardTransactionList[48] = "区域防潜回--拒绝开门";//
                mCardTransactionList[49] = "区域互锁--有门未关好，拒绝开门";//

                mButtonTransactionList[1] = "按钮开门";//
                mButtonTransactionList[2] = "开门时段过期";//
                mButtonTransactionList[3] = "锁定时按钮";//
                mButtonTransactionList[4] = "控制器已过期";//
                mButtonTransactionList[5] = "互锁时按钮(不开门)";//

                mDoorSensorTransactionList[1] = "开门";//
                mDoorSensorTransactionList[2] = "关门";//
                mDoorSensorTransactionList[3] = "进入门磁报警状态";//
                mDoorSensorTransactionList[4] = "退出门磁报警状态";//
                mDoorSensorTransactionList[5] = "门未关好";//

                mSoftwareTransactionList[1] = "软件开门";//
                mSoftwareTransactionList[2] = "软件关门";//
                mSoftwareTransactionList[3] = "软件常开";//
                mSoftwareTransactionList[4] = "控制器自动进入常开";//
                mSoftwareTransactionList[5] = "控制器自动关闭门";//
                mSoftwareTransactionList[6] = "长按出门按钮常开";//
                mSoftwareTransactionList[7] = "长按出门按钮常闭";//
                mSoftwareTransactionList[8] = "软件锁定";//
                mSoftwareTransactionList[9] = "软件解除锁定";//
                mSoftwareTransactionList[10] = "控制器定时锁定";//--到时间自动锁定
                mSoftwareTransactionList[11] = "控制器定时解除锁定";//--到时间自动解除锁定
                mSoftwareTransactionList[12] = "报警--锁定";//
                mSoftwareTransactionList[13] = "报警--解除锁定";//
                mSoftwareTransactionList[14] = "互锁时远程开门";//

                mAlarmTransactionList[1] = "门磁报警";//
                mAlarmTransactionList[2] = "匪警报警";//
                mAlarmTransactionList[3] = "消防报警";//
                mAlarmTransactionList[4] = "非法卡刷报警";//
                mAlarmTransactionList[5] = "胁迫报警";//
                mAlarmTransactionList[6] = "消防报警(命令通知)";//
                mAlarmTransactionList[7] = "烟雾报警";//
                mAlarmTransactionList[8] = "防盗报警";//
                mAlarmTransactionList[9] = "黑名单报警";//
                mAlarmTransactionList[10] = "开门超时报警";//
                mAlarmTransactionList[0x11] = "门磁报警撤销";//
                mAlarmTransactionList[0x12] = "匪警报警撤销";//
                mAlarmTransactionList[0x13] = "消防报警撤销";//
                mAlarmTransactionList[0x14] = "非法卡刷报警撤销";//
                mAlarmTransactionList[0x15] = "胁迫报警撤销";//
                mAlarmTransactionList[0x17] = "撤销烟雾报警";//
                mAlarmTransactionList[0x18] = "关闭防盗报警";//
                mAlarmTransactionList[0x19] = "关闭黑名单报警";//
                mAlarmTransactionList[0x1A] = "关闭开门超时报警";//
                mAlarmTransactionList[0x21] = "门磁报警撤销(命令通知)";//
                mAlarmTransactionList[0x22] = "匪警报警撤销(命令通知)";//
                mAlarmTransactionList[0x23] = "消防报警撤销(命令通知)";//
                mAlarmTransactionList[0x24] = "非法卡刷报警撤销(命令通知)";//
                mAlarmTransactionList[0x25] = "胁迫报警撤销(命令通知)";//
                mAlarmTransactionList[0x27] = "撤销烟雾报警(命令通知)";//
                mAlarmTransactionList[0x28] = "关闭防盗报警(软件关闭)";//
                mAlarmTransactionList[0x29] = "关闭黑名单报警(软件关闭)";//
                mAlarmTransactionList[0x2A] = "关闭开门超时报警";//

                mSystemTransactionList[1] = "系统加电";//
                mSystemTransactionList[2] = "系统错误复位（看门狗）";//
                mSystemTransactionList[3] = "设备格式化记录";//
                mSystemTransactionList[4] = "系统高温记录，温度大于>75";//
                mSystemTransactionList[5] = "系统UPS供电记录";//
                mSystemTransactionList[6] = "温度传感器损坏，温度大于>100";//
                mSystemTransactionList[7] = "电压过低，小于<09V";//
                mSystemTransactionList[8] = "电压过高，大于>14V";//
                mSystemTransactionList[9] = "读卡器接反。";//
                mSystemTransactionList[10] = "读卡器线路未接好。";//
                mSystemTransactionList[11] = "无法识别的读卡器";//
                mSystemTransactionList[12] = "电压恢复正常，小于14V，大于9V";//
                mSystemTransactionList[13] = "网线已断开";//
                mSystemTransactionList[14] = "网线已插入";//
            }

        }
    }
}

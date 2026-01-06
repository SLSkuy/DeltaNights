/* ------------------------------------------------------------
*  Author:  2023051604044 wenrenqiang
 *  Date:  2026.1.6
 *  LastUpdate: 2026.1.6
 *
 *  功能：
 *  - 账号数据读取，暂时用文本处理
 * ------------------------------------------------------------ */
#include "dataload.h"

DataLoad::DataLoad(QObject *parent)
    :QObject(parent)
{


}

void DataLoad::read()
{
    QFile file("../../data.txt");
    if (!file.open(QIODevice::ReadOnly | QIODevice::Text)) {
        return;
    }

    QTextStream in(&file);
    QString line;
    int lineNumber = 0;

    while (!in.atEnd()) {
        line = in.readLine();
        lineNumber++;

        // 跳过空行（包括只包含空白字符的行）
        if (line.trimmed().isEmpty()) {
            //qDebug() << "跳过第" << lineNumber << "行（空行）";
            continue;
        }

        if(line == "玩家账号数据") {
            line = in.readLine();
            quint32 uuid = line.toUInt();
            //qDebug()<<uuid;

            line = in.readLine();
            QString nickname = line;
            //qDebug()<<nickname;

            line = in.readLine();
            QString password = line;
            //qDebug()<<password;

            line = in.readLine();
            QString account = line;
            //qDebug()<<account;

            PlayerInfo* p =new PlayerInfo(uuid,account);
            p->setNickname(nickname);
            p->setPassword(password);
            m_playerInfosByID[uuid]=p;
            m_playerInfosByAccount[account]=p;
        }
        //qDebug()<<"成功读取";
        // 处理非空行
        //qDebug() << "第" << lineNumber << "行：" << line;
        // 你的处理逻辑...
    }
}

void DataLoad::write()
{
    QFile file("../../data.txt");

    // 打开文件：WriteOnly（只写），Text（文本模式）
    if (!file.open(QIODevice::WriteOnly | QIODevice::Text)) {
        qDebug() << "无法打开文件：" << file.errorString();
        return;
    }

    QTextStream out(&file);

    // 写入不同类型的数据
    out << "账号数据\n";
    out << "整数：" << 100 << "\n";
    out << "浮点数：" << 3.14159 << "\n";
    out << "布尔值：" << true << "\n";

    // 使用 Qt::endl（跨平台换行）
    out << "使用Qt换行" << Qt::endl;

    // 格式化写入
    out << QString("姓名：%1，年龄：%2\n").arg("张三").arg(25);

    file.close();
    qDebug() << "文件写入完成";
}

void DataLoad::appendToFile()
{
    QFile file("../../data.txt");

    // Append 模式：在文件末尾添加内容
    if (!file.open(QIODevice::Append | QIODevice::Text)) {
        return;
    }

    QTextStream out(&file);
    QString timestamp = QDateTime::currentDateTime().toString("yyyy-MM-dd HH:mm:ss");
    out << "[" << timestamp << "] 程序启动\n";

    // 或者使用 QIODevice::Append 和 WriteOnly 组合
    // file.open(QIODevice::WriteOnly | QIODevice::Append | QIODevice::Text);
}



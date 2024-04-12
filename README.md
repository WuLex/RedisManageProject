# RedisManageProject
net6 的asp.net core mvc的redis简单管理示例

## 运行结果如图：

<table>
    <tr>
        <td><img src="https://raw.githubusercontent.com/WuLex/UsefulPicture/main/redismanagement/list.png"/></td>
        <td><img src="https://raw.githubusercontent.com/WuLex/UsefulPicture/main/redismanagement/edit.png"/></td>
    </tr>
    <tr>
        <td><img src="https://raw.githubusercontent.com/WuLex/UsefulPicture/main/redismanagement/add.png"/></td>
      
</table>


### 步骤一：创建一个Docker网络
首先，创建一个Docker网络，以便让所有的Redis节点可以相互通信。你可以使用以下命令创建一个自定义网络：

```bash
docker network create redis-net
```

### 步骤二：启动Redis节点
现在，我们将启动多个Redis节点作为我们的集群的一部分。我们将创建一个脚本来简化这个过程。

#### 创建一个 `docker-compose.yml` 文件：

```yaml
version: '3'
services:
  redis-node1:
    image: redis:latest
    container_name: redis-node1
    command: redis-server --appendonly yes --cluster-enabled yes --cluster-config-file nodes.conf --cluster-node-timeout 5000 --port 7001
    networks:
      - redis-net
    ports:
      - "7001:7001"
    volumes:
      - redis-node1-data:/data

  redis-node2:
    image: redis:latest
    container_name: redis-node2
    command: redis-server --appendonly yes --cluster-enabled yes --cluster-config-file nodes.conf --cluster-node-timeout 5000 --port 7002
    networks:
      - redis-net
    ports:
      - "7002:7002"
    volumes:
      - redis-node2-data:/data

  redis-node3:
    image: redis:latest
    container_name: redis-node3
    command: redis-server --appendonly yes --cluster-enabled yes --cluster-config-file nodes.conf --cluster-node-timeout 5000 --port 7003
    networks:
      - redis-net
    ports:
      - "7003:7003"
    volumes:
      - redis-node3-data:/data

  redis-node4:
    image: redis:latest
    container_name: redis-node4
    command: redis-server --appendonly yes --cluster-enabled yes --cluster-config-file nodes.conf --cluster-node-timeout 5000 --port 7004
    networks:
      - redis-net
    ports:
      - "7004:7004"
    volumes:
      - redis-node4-data:/data

  redis-node5:
    image: redis:latest
    container_name: redis-node5
    command: redis-server --appendonly yes --cluster-enabled yes --cluster-config-file nodes.conf --cluster-node-timeout 5000 --port 7005
    networks:
      - redis-net
    ports:
      - "7005:7005"
    volumes:
      - redis-node5-data:/data

  redis-node6:
    image: redis:latest
    container_name: redis-node6
    command: redis-server --appendonly yes --cluster-enabled yes --cluster-config-file nodes.conf --cluster-node-timeout 5000 --port 7006
    networks:
      - redis-net
    ports:
      - "7006:7006"
    volumes:
      - redis-node6-data:/data

networks:
  redis-net:

volumes:
  redis-node1-data:
  redis-node2-data:
  redis-node3-data:
  redis-node4-data:
  redis-node5-data:
  redis-node6-data:
```

#### 启动Redis节点：

```bash
docker-compose up -d
```

这将启动三个Redis节点，分别监听7000、7001和7002端口。

### 步骤三：配置Redis集群
现在，我们需要将这些节点组成一个Redis集群。我们可以使用Redis提供的 `redis-cli` 工具来执行这些操作。

#### 进入一个Redis容器：

```bash
docker exec -it <container_name_or_id> bash
```

#### 使用 `redis-cli` 创建集群：

进入到每个Redis节点容器中，并按照以下步骤执行：

1. 初始化集群

```bash
redis-cli --cluster create \
  <IP-Node1>:7001 \
  <IP-Node2>:7002 \
  <IP-Node3>:7003 \
  --cluster-replicas 1
```
确保用实际的IP地址替换 `<IP-Node1>`, `<IP-Node2>`, `<IP-Node3>`。

或者
```bash
redis-cli --cluster create redis-node1:7001 redis-node2:7002 redis-node3:7003 redis-node4:7004 redis-node5:7005 redis-node6:7006 --cluster-replicas 1
```

这个命令将会问你是否确定创建集群，输入 `yes` 确认。


2. 集群完成后，你可以使用以下命令来验证：

```bash
redis-cli --cluster check <IP-Node1>:7001
```

### 配置完成
现在你已经有一个运行的Redis集群了。你可以通过连接到任何一个节点，例如：

```bash
redis-cli -c -p 7001
```

然后运行 `cluster nodes` 命令来查看集群的节点信息。

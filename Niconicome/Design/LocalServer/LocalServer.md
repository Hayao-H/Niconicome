# ローカルサーバー

## 要件
### 基本機能
- **各種ファイルの配信**
    動画ファイル、css、jsなどローカルファイルの配信。
### 応用機能
- **HLS対応**
    将来的に動画ファイルの配信でHLSに対応する。

## 使用技術
**Model**  
C# (```HttpListener```) 
## URL設計
URL | コンテンツ
-- | --
/niconicome/video/```playlist ID```/```video ID```/video.mp4 | 動画ファイル
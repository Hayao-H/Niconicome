# インポート・エクスポート

## 要件
### 基本機能
- Niconicomeからのデータエクスポート・インポート
- NicomentXenoglossiaからのデータインポート

### 詳細
- エクスポートの際、履歴系は含まない
- Xenoからインポートしたデータはルート直下の``Xeno``プレイリストに追加

## 使用技術
**Model**  
- C#
**Infrastructure**
- C#
- JSON

## JSON定義
[./ExportData.jsonc](./ExportData.jsonc)

## URL設計
URL | コンテンツ
-- | :--:
``/settings/import/`` | インデックス
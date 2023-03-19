# インポート・エクスポート

## 要件
### 基本機能
- Niconicomeからのデータエクスポート・インポート
- NicomentXenoglossiaからのデータインポート

### 詳細
- エクスポートの際、履歴系は含まない

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
``/settings/import/niconicome/import`` | インポート
``/settings/import/niconicome/export`` | エクスポート
``/settings/import/xeno/import`` | インポート(NicomentXenoglossia)
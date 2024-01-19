# Dlls 說明

這個資料夾下的Dlls會在打包時自動產生需要的依賴Dll檔案並打包到Bundle包中。請注意以下事項：

1. **Dlls不進版控：** Dlls底下的Dlls(裡面放每次需打包的Dll檔案)不會進Git版控。
2. **父層的Dlls不可以刪：** Dlls父層是Addressable要設定Lebel用的, 不可以刪除或改名。

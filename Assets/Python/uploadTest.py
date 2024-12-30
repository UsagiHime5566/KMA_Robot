import requests
import os

def upload_image(image_path):
    # API 端點
    url = 'https://kfma.iottalk.tw/api/v1/epaper/1/image'
    
    # 設置 headers
    headers = {
        'Authorization': 'Bearer da31b188fa2949d2b507d520b60e0d14'
    }
    
    try:
        # 讀取圖片文件
        with open(image_path, 'rb') as image_file:
            # 準備文件數據
            files = {
                'file': ('image.jpg', image_file, 'multipart/form-data')
            }
            
            # 發送 POST 請求
            response = requests.post(url, headers=headers, files=files)
            
            # 檢查響應
            if response.status_code == 200:
                print("上傳成功!")
                print("響應內容:", response.text)
            else:
                print(f"上傳失敗! 狀態碼: {response.status_code}")
                print("錯誤信息:", response.text)
                
    except Exception as e:
        print(f"發生錯誤: {str(e)}")

# 使用示例
if __name__ == "__main__":
    # 替換為你的圖片路徑
    image_path = "i:/UserData/UnityProject/KMA_Robot/Assets/Python/test.jpg"
    
    # 檢查文件是否存在
    if os.path.exists(image_path):
        upload_image(image_path)
    else:
        print(f"找不到圖片文件: {image_path}")
<h1 align="center">
    <img src="https://readme-typing-svg.herokuapp.com/?font=Righteous&size=35&center=true&vCenter=true&width=500&height=70&duration=5000&color=F7F7F7FF&background=00000055&lines=Digital+Terrain+Models;" />
</h1>

# Описание проекта
Digital Terrain Models (Цифровые модели рельефа) - программа для анализа обработки поверхности после применения алгоритма. Где содержится 8 алгоритвом такие как: "Butterworth Filter", "Gaussian filter", "Frequency Domain Filter Laplace Filter", "Perfect low and high pass filter", "Laplace filter", "SVD", "DFT" и "Barcode". Результаты выполнения можно отобразить в приложении Unity 3D.

# Пример работы программы
<h1 align="center">
    <img src="assets/DODTM.gif">
</h1>

# Установка
Установка происходит следующим образом:
1) Скачать программу последней версии [тут](https://github.com/DKAVrZoV65F/Digital-Terrain-Models/releases).
2) Распокавать и запустить установочник для **Windows OS**: "DODTM_2.1.0.0_x64.msix" для **MacOS**: "DODTM". Если требуется лицензия, то лицензия лежит в папке рядом с установкой.
3) После установки распоковать программу "DODTM_Algorithms.exe" по следующему пути для **Windows OS**: "C:/Users/User/"; для **MacOS**: "/Users/User/", где "User" - имя пользователя вашей учётной записи.
4) Запустите программу и пользуйтесь.

# Пример использования
**Windows OS** для работы с графическим файлом нужно запустить DODTM.exe, где требуется указать параметры и программа автоматически запустит "DODTM_Algorithms.exe" и после того как закончится выполнение, программа выдаст сообщение об успешной операции.

**MacOS** для работы с графическим файлом нужно запустить DODTM, где требуется указать параметры и программа скопирует команду в буффер обмена для выполнения через командную строку.

Файл "DODTM_Algorithms" содержит 7 алгоритмов "Butterworth Filter", "Gaussian filter", "Frequency Domain Filter Laplace Filter", "Perfect low and high pass filter", "Laplace filter", "SVD" и "DFT". 8й алгоритм содержится в приложении для **Windows OS** - "Barcode".
Запуск производится через командную строку и указываются параметры. 
Для алгоритмов: "Butterworth Filter", "Gaussian filter", "Frequency Domain Filter Laplace Filter", "Perfect low and high pass filter" и "Laplace filter" требуется указать 7 параметров:
1) первый параметр - какой алгоритм применить (по умолчанию DFT);
2) второй параметр - путь до изображения; 
3) третий параметр - куда сохранить изображение;
4) четвёртый параметр - в каком формате сохранить изображение (по умолчанию *.png);
5) пятый параметр - длина изображения (по умолчанию 800);
6) шестой параметр - ширина изображения (по умолчанию 800);
7) седьмой параметр - DPI изображения (по умолчанию 100);

Пример использования:
> ./DODTM_Algorithms "Butterworth Filter" "/Users/User/csgxhjnsqh7j.jpg" "/Users/kamchatka" "png" "800" "800" "100"

Для DFT требуется указать булевский параметр:

8) восьмой параметр - True или False (по умолчанию False);
> ./DODTM_Algorithms "DFT" "/Users/User/csgxhjnsqh7j.jpg" "/Users/kamchatka" "png" "800" "800" "100" "True"

Для SVD требуется указать ещё 3 числовых параметра: 

8) восьмой параметр - шаг (по умолчанию 1);
9) девятый параметр - от скольки (по умолчанию 0);
10) десятый параметр - до скольки (по умолчанию 1);
> ./DODTM_Algorithms "SVD" "/Users/User/csgxhjnsqh7j.jpg" "/Users/kamchatka" "png" "800" "800" "100" "10" "50" "100"

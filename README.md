<h1 align="center">
    <img src="https://readme-typing-svg.herokuapp.com/?font=Righteous&size=35&center=true&vCenter=true&width=500&height=70&duration=5000&color=F7F7F7FF&background=00000055&lines=Digital+Terrain+Models;" />
</h1>

# Описание проекта
Digital Terrain Models (Цифровые модели рельефа) - программа для анализа обработки поверхности после применения алгоритма.

# Пример работы программы
<h1 align="center">
    <img src="assets/DODTM.gif">
</h1>

# Установка
Установка происходит следующим образом:
1) Скачать из программу по ссылке: [https://disk.yandex.ru/d/q79nFG5-GQIpmA](https://disk.yandex.ru/d/q79nFG5-GQIpmA) ;
2) Разархивировать;
3) Открыть папку "DODTM_1.1.0.0._Test", запустить установщик "DODTM_1.1.0.0_x64";
4) Скопировать папку "PROG" и расположить по пути "C:/Users/User/";
5) Запустить программу, если ранее программа была открыта, перезапустите программу;

<h1 align="center">
    <img src="assets/qr.png">
</h1>

# Пример использования
После установки, в папке "C:/Users/User/PROG/" находятся: ButterworthF.exe; DFT.exe; FDFGaussianF.exe; FDFLaplaceF.exe; PLaHPF.exe; SDIFULaplaceF.exe и SVD.exe;
Запуск производится через командную строку и указываются параметры *(для большинство перечисленных программ требуются дополнительные параметры). 
Для программ: ButterworthF.exe; FDFGaussianF.exe; FDFLaplaceF.exe; PLaHPF.exe и SDIFULaplaceF.exe; требуется указать 3 параметра:
1) первый параметр - путь до изображения; 
2) второй параметр - куда сохранить изображение;
3) третий параметр - в каком формате сохранить изображение;

Пример использования:
> C:\Users\User> ButterworthF.exe "C:\Users\User\Pictures\picture.jpg" "C:\Users\User\Documents\" "png"

Для DFT.exe требуется добавить булевский параметр(None или 1):
> C:\Users\User> DFT.exe "C:\Users\User\Pictures\picture.jpg" "C:\Users\User\Documents\" "png" "1"

Для SVD.exe требуется указать 2 числовых параметра: 
1) четвёртый параметр - количество итераций;
2) пятый параметр - шаг;
> C:\Users\User> SVD.exe "C:\Users\User\Pictures\picture.jpg" "C:\Users\User\Documents\" "png" "100" "10"

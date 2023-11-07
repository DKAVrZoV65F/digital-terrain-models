# фильтрует_использование_фурьера_трансформации_dft.
import cv2
from matplotlib import pyplot as plt
import numpy as np
import sys


# imagePath = 'Images/csgxhjnsqh7j.jpg'
# pathToSave = 'C:\\Users\\10\\Documents\\МиВлгу\\PyExec\\ImagesComponents(csgxhjnsqh7j)\\'
# rangeToGen = 10
imagePath = sys.argv[1]
pathToSave = sys.argv[2]
isMask = bool(sys.argv[3])

img = cv2.imread(imagePath, 0)  # загрузить изображение

# Вывод представляет собой двумерный сложный массив. 1- й канал реальный и 2- й воображаемый
# Для бпф в opencv входное изображение должно быть преобразовано в float32
dft = cv2.dft(np.float32(img), flags=cv2.DFT_COMPLEX_OUTPUT)

# Перестраивает преобразование Фурье X путем сдвига нулевой частоты
# компонент в центр массива.
# В противном случае он начинается с верхнего левого корня изображения (массива)
dft_shift = np.fft.fftshift(dft)

# Величина функции равна 20.log(abs(f))
# Для значений, равных 0, мы можем получить неопределенные значения для log.
# Таким образом, мы можем добавить 1 к массиву, чтобы избежать появления предупреждения.
magnitude_spectrum = 20 * np.log(cv2.magnitude(dft_shift[:, :, 0], dft_shift[:, :, 1]))


# Круговая маска HPF, центральный круг равен 0, остальные все единицы
# Может использоваться для определения границ, поскольку низкие частоты в центре блокируются
# и разрешены только высокие частоты. Ребра - это высокочастотные компоненты.
# Шум усилителя.

rows, cols = img.shape
crow, ccol = int(rows / 2), int(cols / 2)

mask = np.ones((rows, cols, 2), np.uint8)
r = 80
center = [crow, ccol]
x, y = np.ogrid[:rows, :cols]
mask_area = (x - center[0]) ** 2 + (y - center[1]) ** 2 <= r*r
mask[mask_area] = 0

# Круговая маска LPF, центральный круг равен 1, остальные все нули
# Допускаются только низкочастотные компоненты - сглаженные области
# Может сгладить шум, но размывает края.
if isMask:
    rows, cols = img.shape
    crow, ccol = int(rows / 2), int(cols / 2)

    mask = np.zeros((rows, cols, 2), np.uint8)
    r = 100
    center = [crow, ccol]
    x, y = np.ogrid[:rows, :cols]
    mask_area = (x - center[0]) ** 2 + (y - center[1]) ** 2 <= r*r
    mask[mask_area] = 1

    # Band Pass Filter - Concentric circle mask, only the points living in concentric circle are ones
    rows, cols = img.shape
    crow, ccol = int(rows / 2), int(cols / 2)

    mask = np.zeros((rows, cols, 2), np.uint8)
    r_out = 80
    r_in = 10
    center = [crow, ccol]
    x, y = np.ogrid[:rows, :cols]
    mask_area = np.logical_and(((x - center[0]) ** 2 + (y - center[1]) ** 2 >= r_in ** 2),
                               ((x - center[0]) ** 2 + (y - center[1]) ** 2 <= r_out ** 2))
    mask[mask_area] = 1

# примените маску и обратный DFT
fshift = dft_shift * mask + 1
fshift_mask_mag = 2000 * np.log(cv2.magnitude(fshift[:, :, 0], fshift[:, :, 1]))

f_ishift = np.fft.ifftshift(fshift)
img_back = cv2.idft(f_ishift)
img_back = cv2.magnitude(img_back[:, :, 0], img_back[:, :, 1])

fig = plt.figure(figsize=(12, 12))
ax1 = fig.add_subplot(2, 2, 1)
ax1.imshow(img, cmap='gray')
ax1.title.set_text('Input Image')
ax2 = fig.add_subplot(2, 2, 2)
ax2.imshow(magnitude_spectrum, cmap='gray')
ax2.title.set_text('FFT of image')
ax3 = fig.add_subplot(2, 2, 3)
ax3.imshow(fshift_mask_mag, cmap='gray')
ax3.title.set_text('FFT + Mask')
ax4 = fig.add_subplot(2, 2, 4)
ax4.imshow(img_back, cmap='gray')
ax4.title.set_text('After inverse FFT')
# plt.show()
plt.savefig(pathToSave + '\DFT.png')

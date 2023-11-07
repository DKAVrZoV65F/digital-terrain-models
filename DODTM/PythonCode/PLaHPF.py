# Идеальный фильтр низких и высоких частот.
import cv2
import numpy as np
import matplotlib.pyplot as plt
import sys


imagePath = sys.argv[1]
pathToSave = sys.argv[2]

# исходное изображение
f = cv2.imread(imagePath, 0)

plt.imshow(f, cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'1. ИсходноеИзображение.png')

# изображение в частотной области
F = np.fft.fft2(f)
plt.imshow(np.log1p(np.abs(F)),
           cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'2. ИзображениеВЧастотнойОбласти.png')

Fshift = np.fft.fftshift(F)
plt.imshow(np.log1p(np.abs(Fshift)),
           cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'3. ИзображениеВЧастотнойОбласти.png')

# Фильтр: Фильтр нижних частот
M, N = f.shape
H = np.zeros((M, N), dtype=np.float32)
D0 = 50
for u in range(M):
    for v in range(N):
        D = np.sqrt((u - M / 2) ** 2 + (v - N / 2) ** 2)
        if D <= D0:
            H[u, v] = 1
        else:
            H[u, v] = 0

plt.imshow(H, cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'4. ФильтрНижнихЧастот.png')

# Идеальная фильтрация нижних частот
Gshift = Fshift * H
plt.imshow(np.log1p(np.abs(Gshift)),
           cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'5. ИдеальнаяФильтрацияНижнихЧастот.png')

# Обратное преобразование Фурье
G = np.fft.ifftshift(Gshift)
plt.imshow(np.log1p(np.abs(G)),
           cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'6. ОбратноеПреобразованиеФурье.png')

g = np.abs(np.fft.ifft2(G))
plt.imshow(g, cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'7. ОбратноеПреобразованиеФурье.png')

# Фильтр: Фильтр высоких частот
H = 1 - H

plt.imshow(H, cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'8. ФильтрВысокихЧастот.png')

# Идеальная фильтрация высоких частот
Gshift = Fshift * H
plt.imshow(np.log1p(np.abs(Gshift)), cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'9. ИдеальнаяФильтрацияВысокихЧастот.png')

# Обратное преобразование Фурье
G = np.fft.ifftshift(Gshift)
plt.imshow(np.log1p(np.abs(G)), cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'10. ОбратноеПреобразованиеФурье.png')

g = np.abs(np.fft.ifft2(G))
plt.imshow(g, cmap='gray')
plt.axis('off')
# plt.show()
plt.savefig(pathToSave + f'11. ОбратноеПреобразованиеФурье.png')

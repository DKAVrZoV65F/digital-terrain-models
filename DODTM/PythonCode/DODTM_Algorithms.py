import cv2
import numpy as np
import matplotlib.pyplot as plt
import sys


algorithm = str(sys.argv[1])
imagePath = str(sys.argv[2])
pathToSave = str(sys.argv[3])
outputFile = str(sys.argv[4])
widthImg = int(sys.argv[5])
heightImg = int(sys.argv[6])
dpiImg = int(sys.argv[7])


fig, ax = plt.subplots()
fig.subplots_adjust(0, 0, 1, 1)
plt.figure(figsize=(widthImg / dpiImg, heightImg / dpiImg), dpi=dpiImg)

if algorithm == "Butterworth Filter":
    f = cv2.imread(imagePath, 0)
    F = np.fft.fft2(f)
    Fshift = np.fft.fftshift(F)
    plt.imshow(np.log1p(np.abs(Fshift)), cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/1. Converting An Image To A Frequency.{outputFile}', transparent=True)
    plt.close(fig)

    M, N = f.shape
    H = np.zeros((M, N), dtype=np.float32)
    D0 = 10
    n = 10
    for u in range(M):
        for v in range(N):
            D = np.sqrt((u - M / 2) ** 2 + (v - N / 2) ** 2)
            H[u, v] = 1 / (1 + (D / D0) ** n)
    plt.imshow(H, cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/2. Butterworth Low-Pass Filter.{outputFile}', transparent=True)
    plt.close(fig)

    Gshift = Fshift * H
    G = np.fft.ifftshift(Gshift)
    g = np.abs(np.fft.ifft2(G))
    plt.imshow(g, cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/3. Image Filters In The Frequency Domain.{outputFile}', transparent=True)
    plt.close(fig)

    HPF = np.zeros((M, N), dtype=np.float32)
    D0 = 10
    n = 1
    for u in range(M):
        for v in range(N):
            D = np.sqrt((u - M / 2) ** 2 + (v - N / 2) ** 2)

            if D == 0:
                D = 1

            HPF[u, v] = 1 / (1 + (D0 / D) ** n)
    plt.imshow(HPF, cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/4. Butterworth High-Pass Filter.{outputFile}', transparent=True)
    plt.close(fig)

    Gshift = Fshift * HPF
    G = np.fft.ifftshift(Gshift)
    g = np.abs(np.fft.ifft2(G))
    plt.imshow(g, cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/5. Image Filters In The Frequency Domain.{outputFile}', transparent=True)
    plt.close(fig)

elif algorithm == "Gaussian filter":
    f = cv2.imread(imagePath, 0)

    plt.imshow(f, cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/1. The Original Image.{outputFile}', transparent=True)
    plt.close(fig)

    F = np.fft.fft2(f)
    Fshift = np.fft.fftshift(F)
    plt.imshow(np.log1p(np.abs(F)), cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/2. Convert The Image To Low-Pass part_1.{outputFile}', transparent=True)
    plt.close(fig)

    plt.imshow(np.log1p(np.abs(Fshift)), cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/3. Convert The Image To Low-Pass part_2.{outputFile}', transparent=True)
    plt.close(fig)

    M, N = f.shape
    H = np.zeros((M, N), dtype=np.float32)
    D0 = 10
    for u in range(M):
        for v in range(N):
            D = np.sqrt((u - M / 2) ** 2 + (v - N / 2) ** 2)
            H[u, v] = np.exp(-D ** 2 / (2 * D0 * D0))

    plt.imshow(H, cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/4. Low-Pass Filter.{outputFile}', transparent=True)
    plt.close(fig)

    Gshift = Fshift * H
    G = np.fft.ifftshift(Gshift)
    g = np.abs(np.fft.ifft2(G))

    plt.imshow(g, cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/5. Low-frequency Image.{outputFile}', transparent=True)
    plt.close(fig)

    plt.imshow(np.log1p(np.abs(Gshift)), cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/6. Convert The Image To High-Pass part_1.{outputFile}', transparent=True)
    plt.close(fig)

    plt.imshow(np.log1p(np.abs(G)), cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/7. Convert The Image To High-Pass part_2.{outputFile}', transparent=True)
    plt.close(fig)

    HPF = 1 - H
    plt.imshow(HPF, cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/8. High-Pass Filter.{outputFile}', transparent=True)
    plt.close(fig)

    Gshift = Fshift * HPF
    G = np.fft.ifftshift(Gshift)
    g = np.abs(np.fft.ifft2(G))

    plt.imshow(g, cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/9. High-Pass Image.{outputFile}', transparent=True)
    plt.close(fig)

elif algorithm == "Frequency Domain Filter Laplace Filter":
    f = cv2.imread(imagePath, 0)
    f = f / 255
    plt.imshow(f, cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/1. The Original Image.{outputFile}', transparent=True)
    plt.close(fig)

    F = np.fft.fftshift(np.fft.fft2(f))
    plt.imshow(np.log1p(np.abs(F)), cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/2. Conversion To The Frequency Domain.{outputFile}', transparent=True)
    plt.close(fig)

    P, Q = F.shape
    H = np.zeros((P, Q), dtype=np.float32)
    for u in range(P):
        for v in range(Q):
            H[u, v] = -4 * np.pi * np.pi * ((u - P / 2) ** 2 + (v - Q / 2) ** 2)
    plt.imshow(H, cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/3. Laplace Filter.{outputFile}', transparent=True)
    plt.close(fig)

    Lap = H * F
    Lap = np.fft.ifftshift(Lap)
    Lap = np.real(np.fft.ifft2(Lap))
    OldRange = np.max(Lap) - np.min(Lap)
    NewRange = 1 - -1
    LapScaled = (((Lap - np.min(Lap)) * NewRange) / OldRange) + -1

    plt.imshow(LapScaled, cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/4. Convert The Value Of The Laplace Image.{outputFile}', transparent=True)
    plt.close(fig)

    c = -1
    g = f + c * LapScaled
    g = np.clip(g, 0, 1)

    plt.imshow(g, cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/5. Execution result.{outputFile}', transparent=True)
    plt.close(fig)

elif algorithm == "Perfect low and high pass filter":
    f = cv2.imread(imagePath, 0)
    plt.imshow(f, cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/1. The Original Image.{outputFile}', transparent=True)
    plt.close(fig)

    F = np.fft.fft2(f)
    plt.imshow(np.log1p(np.abs(F)),
               cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/2. Converting An Image To A Frequency part_1.{outputFile}', transparent=True)
    plt.close(fig)

    Fshift = np.fft.fftshift(F)
    plt.imshow(np.log1p(np.abs(Fshift)),
               cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/3. Converting An Image To A Frequency part_2.{outputFile}', transparent=True)
    plt.close(fig)

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
    plt.savefig(pathToSave + f'/4. Converting An Image To A Low-Pass part_1.{outputFile}', transparent=True)
    plt.close(fig)

    Gshift = Fshift * H
    plt.imshow(np.log1p(np.abs(Gshift)),
               cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/5. Converting An Image To A Low-Pass part_2.{outputFile}', transparent=True)
    plt.close(fig)

    G = np.fft.ifftshift(Gshift)
    plt.imshow(np.log1p(np.abs(G)),
               cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/6. Converting An Image To A Low-Pass part_3.{outputFile}', transparent=True)
    plt.close(fig)

    g = np.abs(np.fft.ifft2(G))
    plt.imshow(g, cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/7. Low-Pass Image.{outputFile}', transparent=True)
    plt.close(fig)

    H = 1 - H
    plt.imshow(H, cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/8. Converting An Image To A High-Pass part_1.{outputFile}', transparent=True)
    plt.close(fig)

    Gshift = Fshift * H
    plt.imshow(np.log1p(np.abs(Gshift)), cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/9. Converting An Image To A High-Pass part_2.{outputFile}', transparent=True)
    plt.close(fig)

    G = np.fft.ifftshift(Gshift)
    plt.imshow(np.log1p(np.abs(G)), cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/10. Converting An Image To A High-Pass part_3.{outputFile}', transparent=True)
    plt.close(fig)

    g = np.abs(np.fft.ifft2(G))
    plt.imshow(g, cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/11. High-Pass Image.{outputFile}', transparent=True)
    plt.close(fig)

elif algorithm == "Laplace filter":
    img = cv2.imread(imagePath, 0)
    plt.imshow(img, cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/1. The Original Image.{outputFile}', transparent=True)
    plt.close(fig)

    kernel = np.array([[0, 1, 0],
                       [1, -4, 1],
                       [0, 1, 0]])

    LaplacianImage = cv2.filter2D(src=img,
                                  ddepth=-1,
                                  kernel=kernel)
    plt.imshow(LaplacianImage, cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/2. Low-Pass Image.{outputFile}', transparent=True)
    plt.close(fig)

    c = -1
    g = img + c * LaplacianImage
    plt.imshow(g, cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/3. Process Images.{outputFile}', transparent=True)
    plt.close(fig)

    gClip = np.clip(g, 0, 255)
    plt.imshow(gClip, cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/4. High-Pass Image.{outputFile}', transparent=True)
    plt.close(fig)

elif algorithm == "SVD":
    step = int(sys.argv[8])
    startRange = int(sys.argv[9])
    endRange = int(sys.argv[10])

    img = cv2.imread(imagePath)
    gray_image = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
    u, s, v = np.linalg.svd(gray_image, full_matrices=False)
    if endRange > int(s.shape[0]):
        endRange = int(s.shape[0])
    comps = []
    for i in range(startRange, endRange, step):
        comps.append(i)
    for i in range(len(comps)):
        low_rank = u[:, :comps[i]] @ np.diag(s[:comps[i]]) @ v[:comps[i], :]
        plt.imshow(low_rank, cmap='gray')
        plt.axis('off')
        plt.savefig(pathToSave + f'/Actual Image With Component={comps[i]}.{outputFile}', transparent=True)
        plt.close(fig)

else:
    isMask = bool(sys.argv[8])

    img = cv2.imread(imagePath, 0)
    dft = cv2.dft(np.float32(img), flags=cv2.DFT_COMPLEX_OUTPUT)
    dft_shift = np.fft.fftshift(dft)
    magnitude_spectrum = 20 * np.log(cv2.magnitude(dft_shift[:, :, 0], dft_shift[:, :, 1]))
    rows, cols = img.shape
    crow, ccol = int(rows / 2), int(cols / 2)

    mask = np.ones((rows, cols, 2), np.uint8)
    r = 80
    center = [crow, ccol]
    x, y = np.ogrid[:rows, :cols]
    mask_area = (x - center[0]) ** 2 + (y - center[1]) ** 2 <= r * r
    mask[mask_area] = 0

    if isMask:
        rows, cols = img.shape
        crow, ccol = int(rows / 2), int(cols / 2)

        mask = np.zeros((rows, cols, 2), np.uint8)
        r = 100
        center = [crow, ccol]
        x, y = np.ogrid[:rows, :cols]
        mask_area = (x - center[0]) ** 2 + (y - center[1]) ** 2 <= r * r
        mask[mask_area] = 1

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

    fshift = dft_shift * mask + 1
    fshift_mask_mag = 2000 * np.log(cv2.magnitude(fshift[:, :, 0], fshift[:, :, 1]))

    f_ishift = np.fft.ifftshift(fshift)
    img_back = cv2.idft(f_ishift)
    img_back = cv2.magnitude(img_back[:, :, 0], img_back[:, :, 1])

    plt.imshow(img, cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/1. The Original Image.{outputFile}', transparent=True)
    plt.close(fig)

    plt.imshow(magnitude_spectrum, cmap='gray')
    plt.axis('off')
    plt.savefig(pathToSave + f'/2. FFT Of Image.{outputFile}', transparent=True)
    plt.close(fig)

    if isMask:
        plt.imshow(fshift_mask_mag, cmap='gray')
        plt.axis('off')
        plt.savefig(pathToSave + f'/3. FFT With Mask.{outputFile}', transparent=True)
        plt.close(fig)

        plt.imshow(img_back, cmap='gray')
        plt.axis('off')
        plt.savefig(pathToSave + f'/4. After Inverse FFT With Mask.{outputFile}', transparent=True)
        plt.close(fig)
    else:
        plt.imshow(fshift_mask_mag, cmap='gray')
        plt.axis('off')
        plt.savefig(pathToSave + f'/3. FFT Without Mask.{outputFile}', transparent=True)
        plt.close(fig)

        plt.imshow(img_back, cmap='gray')
        plt.axis('off')
        plt.savefig(pathToSave + f'/4. After Inverse FFT Without Mask.{outputFile}', transparent=True)
        plt.close(fig)


'''
                   ``
  `.              `ys
  +h+             +yh-
  yyh:           .hyys
 .hyyh.          oyyyh`
 /yyyyy`        .hyydy/
 syyhhy+        oyyhsys
 hyyyoyh.      .hyyy:hh`
.hyyyy:ho      +yyys-yh-
:hyyyh-oh.    `hyyyo-oy/
/yyyyh-:h+    -hyyh/-oy+
+yyyyh:-yy    +yyyh--oyo
+yyyyh/-sh.   syyyh--oyo
+yyyyh/-oy/  `hyyyy--syo
+yyyyh/-+y+  `hyyys--yy+
:yyyyh/-+ys  .hyyyo-:hy:
.hyyyh+-+ys  .hyyyo-oyh`
`yyyyyo-oyy  .hyyy+-yyy
 +yyyys-syy  `hyyh/oyy/
 .hyyyh-hyy  `hyyh/hyh
  oyyyhshys   yyyhyyy+
  oyyyhshys   yyyhyyy+
   /hyyyyyo`.-oyyyyh/
   `syyyyyyyhyyyyyyho.
    .hyyyyhNdyyyyyyymh/`
'''

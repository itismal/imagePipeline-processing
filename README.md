# Image Pipline Processing (IPP)

## Overview

The project aims on processing an image or a pipeline of images (**folder** of image/s) through a series of node/s (image filters). Following nodes can be implemented on choice (via a **pipe file**):  

* Vignette
* Noising (0-100%)
* Convolution
  * Blur
  * Edge
  * Sharpen

## Process

Node/s are entered in the pipe file. Upon execution:

* If a single node is entered, then image/s will be processed in that node. Upon saving, processed image of orignal image/s will be formed as final image/s.
* If multiple nodes are entered, then image/s will be processed in the first node and the output will be input of following node until last node. Upon saving, (**chain**) processed image (from all nodes) of original image/s will be formed as final image/s.
  * If -saveall argument was applied, then all intermediate images will be saved with the final processed image.

## Resources Information

**IDE:** [Microsoft Visual Studio](https://visualstudio.microsoft.com/)  
**Framework:** .Net Core 3.1  
**Application/Output Type:** Console Application  

**Library:**  SixLabors.ImageSharp [v1.0.0-rc0003](https://www.nuget.org/packages/SixLabors.ImageSharp)  

Read more about ***SixLabors.ImageSharp*** [here](https://docs.sixlabors.com/articles/imagesharp/index.html?tabs=tabid-1).  
***SixLabors.ImageSharp*** [Source Repository](https://github.com/SixLabors/ImageSharp)  

## Pipe File

*pipe.txt* file will be available under:  
>IPP\IPP\bin\Debug\netcoreapp3.1

Pipe file contains description of usage in itself. Please read before implementation.

## Implementation

*IPP.exe* file will be available under:  
>IPP\IPP\bin\Debug\netcoreapp3.1

Execution can be done via Command Prompt or Visual Studio Debug Properties of the project.  

Following arguments are used for implementation:

* **Mandatory arguments:**
  * -input \<>
  * -pipe \<>
  * -output \<>

* **Optional arguments:**
  * -verbose
  * -saveall

**-input:** specifies path of an image or a folder of image/s. Usages:  
>When image is in **netcoreapp3.1** folder  
>**`-input image_name.png`**  
>When image_folder is in **netcoreapp3.1** folder  
>**`-input image_folder_name`**  
>Add path if image/image_folder is not located in **netcoreapp3.1** folder  
>**`-input path/image_name.png`** **OR** **`-input path/image_folder_name`**

**-pipe:** specifies path of pipe file (.txt file). Usages:  
>When pipe file is in **netcoreapp3.1** folder  
>**`-pipe pipe-file.txt`**  
>Add path if pipe file is not located in **netcoreapp3.1** folder  
>**`-pipe path/pipe-file.txt`**

**-output:** specifies path of output folder. Usages:  
>If no output folder is specified, program saves images in **netcoreapp3.1** folder by default  
>**`-output`**  
>If folder name is specified, it will be saved under **netcoreapp3.1** folder by default  
>**`-output output_folder_name`**  
>For saving on desired location, add path  
>**`-output path/output_folder_name`**

**-verbose:** logs following on command line:

* Node name
* Node processing time
* Image input size
* Image output size

**-saveall:** saves all intermediate node if applicable

**Usage (regardless of the order of arguments):**
>IPP.exe -input \<> -pipe \<> -output \<> -saveall -verbose

## Progress

[x] Implementation as in first version  
[ ] Allowing user to enter/choosing pipe data without pipefile  
[ ] Detailed README.md with examples  
[ ] New image processing methods

## Contribution Statement

Anyone is allowed to contribute in the project. Before doing so, please open up an issue for discussion or contact me via email <malavpatel501@gmail.com>. And also let me know if you do not know how to contribute (start by cloning or better, forking my master branch).  

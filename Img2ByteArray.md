#C# converting image to byte[.md](.md) and vice versa

# Converting an image to a byte array #

Found this code snippet of a method to convert an image file to an array of bytes. I figured I'd post it here in case anyone else needed it.

```
public byte[] imageToByteArray(System.Drawing.Image imageIn)
{
 MemoryStream ms = new MemoryStream();
 imageIn.Save(ms,System.Drawing.Imaging.ImageFormat.Gif);
 return  ms.ToArray();
}
```

```
public Image byteArrayToImage(byte[] byteArrayIn)
{
     MemoryStream ms = new MemoryStream(byteArrayIn);
     Image returnImage = Image.FromStream(ms);
     return returnImage;
}
```

## Source ##

http://www.codeproject.com/KB/recipes/ImageConverter.aspx
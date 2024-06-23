# Second Shooter

## Summary

This is a set of tools designed to manage photo collections

## Tool List

- [ ] Deduplication
- [ ] Object Detection
- [ ] Histograms
  - [ ] Per Channel RGB
- [ ] RGB to HSV
- [ ] Image Conversion
- [ ] Conversion to B&W
- [ ] EXIF Database
- [ ] Metadata Management
- [ ] Lightroom Integration
- [ ] Image resizing

## Data Process

```plantuml

actor User
boundary WebAPI
entity ImageService
control TypeDetector
control TypeConverter
control ImageTiler
database MetaStore
entity EmbeddingService
control ObjectDetector
entity ClassficationService

User -> WebAPI : Upload File
  activate WebAPI
    WebAPI -\ ImageService : StoreImage
      activate ImageService
      
        ImageService -> TypeDetector : CheckType(content)
          activate TypeDetector
          return contentType

        ImageService -> TypeConverter : ConvertTo(targetContentType)
          activate TypeConverter
          return convertedContent

        ImageService -\ ImageTiler : GetTiles(convertedContent)
        
        ImageService -> MetaStore : StoreMetaData(content)
          activate MetaStore
          return
          
        ImageService -> EmbeddingService : GetImageEmbedding(content)
          activate EmbeddingService
          return imageVector
          
        ImageService -> ClassficationService : GetClassification(content)
          activate ClassficationService
          return description
          
        ImageService -> EmbeddingService : GetSentenceEmbedding(description)
          activate EmbeddingService
          return descriptionVector
          
        ImageService -> MetaStore : StoreVector(contentRef, imageVector, desciption, descriptionVector )
          activate MetaStore
          return
          
        ImageService -> ObjectDetector : DetectObjects(content)
          activate ObjectDetector
          return [{top,left,height,width,type}]

        loop foreach object
        
          ImageService -> EmbeddingService : GetImageEmbedding(objectContent)
            activate EmbeddingService
            return vector
            
          ImageService -> ClassficationService : GetClassification(objectContent)
            activate ClassficationService
            return description
            
          ImageService -> EmbeddingService : GetSentenceEmbedding(description)
            activate EmbeddingService
            return vector
            
          ImageService -> MetaStore : StoreVector(objectContentRef, imageVector, desciption, descriptionVector )
            activate MetaStore
            return
        end
```
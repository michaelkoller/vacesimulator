The most important fields are
startFrame
endFrame
iaMat
attributeMap
subject
dish

				frameRate: Frame rate of all videos
                  dataset: 'cooking12eccv'
		     attributeMap: Name of attributes
                   fileId: int id of files, see fileName
         startTimeSeconds: start time in seconds, with respect to the video
               startFrame: start frame, with respect to the video
           endTimeSeconds: end time of in seconds, with respect to the video
                 endFrame: end frame, with respect to the video
                 activity: more annotation details, relevant is iaMat
                     tool: more annotation details, relevant is iaMat
           toolProperties: more annotation details, relevant is iaMat
              ingredients: more annotation details, relevant is iaMat
    ingredientsProperties: more annotation details, relevant is iaMat
               containers: more annotation details, relevant is iaMat
     containersProperties: more annotation details, relevant is iaMat
         containersSource: more annotation details, relevant is iaMat
    containersDestination: more annotation details, relevant is iaMat
                   fields: fields which are used
               attrFields: fields which are used
                    iaMat: label matrix of attributes
                  subject: id of human subject, determines training, val, test split
                     dish: dish/topic/composite id
                 fileName: string identifying the video file, sSS-dDD-cam-002 with SS=subject and DD==dish
               fileNameId: string identifying the video file without camera info, sSS-dDD with SS=subject and DD==dish
              minBgWindow: only set for detection
              bgClassName: background class Name for detection
                  bgLabel: class label id of background class (only for detection)
              bgLabelAttr: attribute label id of background class (only for detection)
             nImgsPerFile: number of frames per video
                  filters: ignore, string replacements to clean up annotations (already applied to this data)
                idxInFile: the index of the annotation in the corresponding video.
                 nullAttr: ignore
            nullAttrLabel: ignore

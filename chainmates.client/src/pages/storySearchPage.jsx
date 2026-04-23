import React, { useEffect, useState } from "react";
import { useParams } from 'react-router-dom';

import { PageOrTabLayout } from "../layouts/layouts";
import { contactAPI } from "../supportFuncs/utilityFuncs";

import { SegmentDisplayInModal } from "../segmentDisplay"

export default { StorySearchPage};

export function StorySearchPage() {

    const { storyID } = useParams();
    const [storyInfo, setStoryInfo] = useState(null);
    const [finalSegmentDTO, setFinalSegmentDTO] = useState(null);

    async function changeFinalSegment(finalSegmentID) {
        await contactAPI(`segments/${finalSegmentID}/history`, "get")
            .then(function (value) {
                setFinalSegmentDTO(value);
            })
    }

    useEffect(() => {
        const fetchData = async (storyID) => {
            await contactAPI(`stories/${storyID}`, "get", true)
                .then(function (value) {
                    setStoryInfo(value);
                    console.log(value);
                    changeFinalSegment(value.structure[0][0]); // The first segment in the story is saved at 0 (in an array of one)
                    //navigate(`/stories/${storyID}/${firstSegmentID}/`);
                });
        }
        console.log("In useeffect")
        if (storyID) {
            fetchData(storyID);
        }
    }, [storyID]);



    ///////////////// Assign each segment an on-off selection for the Comments + Info sidebar ///////////////
    let noSelections = {};
    console.log(finalSegmentDTO)
    if (finalSegmentDTO) {
        finalSegmentDTO.segmentHistoryList.forEach(dictInArray =>
            noSelections[dictInArray.id] = false);
    }
    const [selectedSegmentDict, setSelectedSegmentDict] = useState(noSelections);
    function changeSegmentSelection(segmentID) {
        setSelectedSegmentDict({ ...selectedSegmentDict, [segmentID]: !selectedSegmentDict[segmentID] })
    }

    if (!storyInfo) {
        if (storyInfo != null) {
            console.log("DIDN@T WORK");
        }
        return null;
    }

    return (
        <PageOrTabLayout 
            topLine={
                <StorySeachPageTopLine storyInfo={storyInfo} />
            }
            mainContent={
                <SegmentSeriesDisplay segmentHistoryList={finalSegmentDTO.segmentHistoryList}
                    editableID={null}
                    currentContent={null}
                    changeSelection={changeSegmentSelection}
                    onChange={null}
                /> 
            }
            footer={
                <>
                    <GoUpASegmentButton previousSegmentID={finalSegmentDTO.segmentHistoryList[-2].id}
                        changeFinalSegment={changeFinalSegment} />
                    <GoDownASegmentButton options={structure[finalSegmentDTO.id]}
                        changeFinalSegment={changeFinalSegment}/>
                </>
            }
            rightSidebar={
                <Comments selections={selectedSegmentDict} storyDict={storyInfo} />
            
            }
        /> 
    )
}

function StorySeachPageTopLine() { }

function GoDownASegmentButton({ possibleSegmentIDList }) {
    const [isOpen, setIsOpen] = useState(false);
    const [arrayOfAvailableSegments, setArrayOfAvailableSegments] = useState([]);

    function createModal() {
        setIsOpen(true);
        console.log("modal clicked")
    }

    useEffect(() => {
        async function getSegmentsForModal(possibleSegmentIDList) {
            let possibleSegmentDTOList;
            let segmentHistoryDTO;
            await Promise.all(possibleSegmentIDList.map(async (segmentID) => {
                segmentHistoryDTO = await contactAPI(`segments/${segmentID}/history`, "get", true);
                possibleSegmentDTOList.push(segmentHistoryDTO);
            }
            )
            )
            await setArrayOfAvailableSegments(segmentHistoryDTOArray);
            return segmentHistoryDTOArray;
        }

        getSegmentsForModal(possibleSegmentIDList)
    },[possibleSegmentIDList])

    return (
        <>
            <button onClick={createModal}> Continue Reading...
            </ button >
            <ModalWindow isOpen={isOpen} onClose={() => setIsOpen(false)}>
                <div className="allDisplayStoriesContainer">
                    {arrayOfAvailableSegments.map(availableStory =>
                        <SegmentDisplayInModal
                            key={availableSegment.id}
                            selectStory={() => props.changeFinalSegment(availableSegment.id)}
                            storyDict={availableSegment}
                        />
                    )}
                </div>
            </ModalWindow >
        </>
    )

}

function GoUpASegmentButton({previousSegmentID}) {

    return (
        <button onClick={() => props.changeFinalSegment(penultimateSegmentID)}/>
    )
}




//export function StorySubSearchPage() {
//    const { finalSegmentID } = useParams();

//    return (
//        <SegmentSeriesDisplay storyDict={storyDict}
//            editableID={null}
//            currentContent={null}
//            changeSelection={changeSegmentSelection}
//            onChange={null}
//        />
//    )
//}
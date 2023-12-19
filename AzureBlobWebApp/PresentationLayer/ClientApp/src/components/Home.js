import React from 'react';
import FileItem from './FileItem';

const Home = (props) => {



  return (
    <div className='d-flex flex-row flex-wrap'>
      <FileItem name="test name 1" type="img" />
      <FileItem name="test name 2" type="video" />
    </div>
  );
}

export default Home
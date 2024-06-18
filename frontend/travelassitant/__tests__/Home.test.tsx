import React from 'react'
import {render, screen} from '@testing-library/react'
import Home from '../app/page'

it('renders page elements correctly', async () =>{
    render(<Home />); // ARRANGE
    const myelem = screen.getByText('Travel Assistant'); // ACT
    expect(myelem).toBeInTheDocument(); // ASSERT  

})

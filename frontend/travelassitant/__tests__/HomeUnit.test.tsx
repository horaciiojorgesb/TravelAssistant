import React from 'react'
import {render, screen} from '@testing-library/react'
import Home from '../app/page'

jest.mock('../app/CountriesSearcher', () =>
{
   return () => <div data-testid="mock-contries-searcher">Mock ContriesSearcher</div>;
}
)

it('renders page elements correctly', async () =>{
    render(<Home />); // ARRANGE
    const myelem = screen.getByText('Travel Assistant'); // ACT
    expect(myelem).toBeInTheDocument(); // ASSERT  
})

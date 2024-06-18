import {render, screen} from '@testing-library/react'
import Home from '../app/page'
import { it } from 'node:test'

it('should have Travel Assistant text', () =>{
    render(<Home />) // ARRANGE
    const myelem = screen.getByText('Travel Assistant') // ACT

    expect(myelem).toBeInTheDocument() // ASSERT
})
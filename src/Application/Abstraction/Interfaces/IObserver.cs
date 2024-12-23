﻿using Domain.Orders;

namespace Application.Abstraction.Interfaces;

public interface IObserver
{
    void Update(string message);
}
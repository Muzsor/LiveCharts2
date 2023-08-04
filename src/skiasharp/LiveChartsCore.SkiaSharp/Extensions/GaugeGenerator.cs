﻿// The MIT License(MIT)
//
// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LiveChartsCore.Defaults;

namespace LiveChartsCore.SkiaSharpView.Extensions;

public static class GaugeGenerator
{
    /// <summary>
    /// Builds a Gauge, it generates a series collectio of
    /// <see cref="PieSeries{ObservableValue, DoughnutGeometry, LabelGeometry}"/>, these series
    /// are ready to be plotted in a pie chart, and will render the gauge, this reuses all the power and
    /// functionality of the <see cref="PieChart{TDrawingContext}"/> class.
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    public static ObservableCollection<PieSeries<ObservableValue>> Build(
        params GaugeItem[] items)
    {
        List<GaugeItem> seriesRules = new();
        List<GaugeItem> backgroundRules = new();

        foreach (var item in items)
        {
            if (item.IsFillSeriesBuilder)
                backgroundRules.Add(item);
            else
                seriesRules.Add(item);
        }

        var count = seriesRules.Count;
        var i = 0;

        var series = new ObservableCollection<PieSeries<ObservableValue>>(
            seriesRules.Select(item =>
            {
                Action<ObservableValue, PieSeries<ObservableValue>>? l =
                    item.Builder is null ? null : (m, s) => { item.Builder?.Invoke(s); };

                return PieChartExtensions.AsSeries<ObservableValue, PieSeries<ObservableValue>>(
                    item.Value,
                    l ?? ((x, x1) => { }),
                    i++,
                    count,
                    GaugeOptions.Gauge);
            }));

        var backgroundSeries = new PieSeries<ObservableValue>(true, true)
        {
            ZIndex = -1,
            IsFillSeries = true,
            Values = new ObservableValue[] { new(0) }
        };

        foreach (var rule in backgroundRules)
            rule.Builder?.Invoke(backgroundSeries);

        series.Add(backgroundSeries);

        return series;
    }
}
